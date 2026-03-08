using System;
using System.Collections;
using Common;
using Player.ActionStates;
using Player.MovementStates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Weapon;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Dying = Animator.StringToHash("Dying");

        [SerializeField] private float _runSpeed = 5f;
        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private ushort _availableJumpCount = 2;
        [SerializeField] private float _coyoteJumpTime = 0.2f;
        [SerializeField] private float _climbSpeed = 5f;
        [SerializeField] private float _climbBlockTime = 1f;
        [SerializeField] private Tilemap _climbingTilemap;
        [SerializeField] private Vector2 _knockbackForce = new(3f, 8f);
        [SerializeField] private float _knockbackTime = 0.5f;
        [SerializeField] private ParticleSystem _bloodParticles;

        private bool _canMove = true;
        private float _lastClimbTime;
        private float _lastGroundedTime;
        private PlayerHealthController _healthController;
        private PlayerLegs _legs;
        private WaitForSeconds _knockbackRoutine;

        public float RunSpeed => _runSpeed;
        public float ClimbSpeed => _climbSpeed;
        public float JumpForce => _jumpForce;
        public ushort AvailableJumpCount => _availableJumpCount;

        public Vector2 CurrentClimbPosition { get; private set; } = Vector2.zero;
        public uint Coins { get; private set; }

        public Rigidbody2D RB { get; private set; }
        public Animator Anim { get; private set; }
        public PlayerInputHandler Input { get; private set; }
        public BulletController BulletController { get; private set; }

        public PlayerStateMachine MovementFSM { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerRunState RunState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerClimbState ClimbState { get; private set; }

        public PlayerStateMachine ActionFSM { get; private set; }
        public PlayerNoneState NoneState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }

        public UnityEvent Died { get; } = new();

        private void Awake()
        {
            _healthController = GetComponent<PlayerHealthController>();
            _legs = GetComponentInChildren<PlayerLegs>();
            _knockbackRoutine = new WaitForSeconds(_knockbackTime);

            RB = GetComponent<Rigidbody2D>();
            Anim = GetComponent<Animator>();
            Input = GetComponent<PlayerInputHandler>();
            BulletController = GetComponent<BulletController>();

            SpriteFlipper spriteFlipper = new(transform.localScale.x);

            MovementFSM = new PlayerStateMachine();
            IdleState = new PlayerIdleState(MovementFSM, this);
            RunState = new PlayerRunState(MovementFSM, this, spriteFlipper);
            JumpState = new PlayerJumpState(MovementFSM, this, spriteFlipper);
            ClimbState = new PlayerClimbState(MovementFSM, this);

            ActionFSM = new PlayerStateMachine();
            NoneState = new PlayerNoneState(ActionFSM, this);
            AttackState = new PlayerAttackState(ActionFSM, this);
        }

        private void Start()
        {
            MovementFSM.Initialize(IdleState);
            ActionFSM.Initialize(NoneState);
        }

        private void OnEnable()
        {
            _healthController.OnDamageTook.AddListener(TakeDamage);
            _healthController.OnDeath.AddListener(Die);
        }

        private void OnDisable()
        {
            _healthController.OnDamageTook.RemoveListener(TakeDamage);
            _healthController.OnDeath.RemoveListener(Die);
        }

        private void Update()
        {
            if (_canMove)
            {
                MovementFSM.Update();
                ActionFSM.Update();
            }
        }

        private void FixedUpdate()
        {
            if (_canMove)
            {
                MovementFSM.FixedUpdate();
                ActionFSM.FixedUpdate();
            }

            if (IsGrounded())
            {
                _lastGroundedTime = Time.time;

                if (Mathf.Abs(Input.MoveInput.x) < Mathf.Epsilon && MathF.Abs(RB.linearVelocityX) > 0.1f)
                {
                    RB.linearVelocityX = 0f;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Climbing))
            {
                Vector3Int cellPosition = _climbingTilemap.WorldToCell(transform.position);
                CurrentClimbPosition = _climbingTilemap.GetCellCenterWorld(cellPosition);
            }
            else if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Enemy)
                     && _legs.IsOnTheLayer(LayerMaskProvider.Enemy))
            {
                Knockback(other.gameObject, false);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Climbing))
            {
                CurrentClimbPosition = Vector2.zero;
            }
        }

        public bool CanJumpCoyote()
        {
            return Time.time < _lastGroundedTime + _coyoteJumpTime;
        }

        public bool CanClimb()
        {
            return CurrentClimbPosition != Vector2.zero && Time.time > _lastClimbTime + _climbBlockTime;
        }

        public void BlockClimbTemporary()
        {
            _lastClimbTime = Time.time;
        }

        public bool IsGrounded()
        {
            return !(Mathf.Abs(RB.linearVelocityY) > 0.1f) && _legs.IsOnTheLayer(LayerMaskProvider.Ground);
        }

        public void AddCoins(uint count)
        {
            Coins += count;
        }

        private void TakeDamage(GameObject other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Enemy))
            {
                Knockback(other);
            }
        }

        private void Knockback(GameObject other, bool disableMovement = true)
        {
            float otherDirectionX = transform.position.x - other.transform.position.x;
            float knockbackDirectionX = otherDirectionX > 0 ? 1 : -1;
            RB.linearVelocity = new Vector2(knockbackDirectionX * _knockbackForce.x, _knockbackForce.y);

            if (disableMovement)
            {
                _canMove = false;
                StartCoroutine(KnockbackRoutine());
            }
        }

        private IEnumerator KnockbackRoutine()
        {
            yield return _knockbackRoutine;

            _canMove = true;
        }

        private void Die(GameObject other)
        {
            enabled = false;
            gameObject.layer = LayerMaskProvider.MaskToLayer(LayerMaskProvider.DeadPlayer);

            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Water))
            {
                RB.gravityScale = 0.2f;
                RB.linearDamping = 5f;
            }

            if (!LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Water))
            {
                _bloodParticles.Play(true);
            }

            Anim.SetTrigger(Dying);
            StartCoroutine(FinalizeDeathRoutine());
        }

        private IEnumerator FinalizeDeathRoutine()
        {
            while (!IsGrounded())
            {
                yield return null;
            }

            RB.simulated = false;
            Died.Invoke();
        }
    }
}
