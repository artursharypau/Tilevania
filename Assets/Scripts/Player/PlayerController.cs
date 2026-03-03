using System.Collections;
using Common;
using Player.ActionStates;
using Player.MovementStates;
using UnityEngine;
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
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = Mathf.Epsilon;
        [SerializeField] private ParticleSystem _bloodParticles;
        [SerializeField] private Vector2 _deathKick = new(5f, 10f);

        private float _lastClimbTime;
        private float _lastGroundedTime;
        private PlayerHealthController _healthController;

        public float RunSpeed => _runSpeed;
        public float ClimbSpeed => _climbSpeed;
        public float JumpForce => _jumpForce;
        public ushort AvailableJumpCount => _availableJumpCount;
        public Vector2 CurrentClimbPosition { get; private set; } = Vector3.zero;

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
        public PlayerNoneState NoneState { get; set; }
        public PlayerAttackState AttackState { get; set; }

        private void Awake()
        {
            _lastClimbTime = 0f;
            _lastGroundedTime = 0f;
            _healthController = GetComponent<PlayerHealthController>();

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
            _healthController.OnDamageTook.AddListener(TakeDamage);
            _healthController.OnDeath.RemoveListener(Die);
        }

        private void Update()
        {
            MovementFSM.Update();
            ActionFSM.Update();
        }

        private void FixedUpdate()
        {
            MovementFSM.FixedUpdate();
            ActionFSM.FixedUpdate();

            if (IsGrounded())
            {
                _lastGroundedTime = Time.time;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Climbing))
            {
                Vector3Int cellPosition = _climbingTilemap.WorldToCell(transform.position);
                CurrentClimbPosition = _climbingTilemap.GetCellCenterWorld(cellPosition);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Climbing))
            {
                CurrentClimbPosition = Vector2.zero;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
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
            return Mathf.Abs(RB.linearVelocity.y) > 0.1f
                ? false
                : Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, LayerMaskProvider.Ground);
        }

        private void TakeDamage()
        {
            _bloodParticles.Play(true);
        }

        private void Die(GameObject other)
        {
            enabled = false;
            gameObject.layer = LayerMaskProvider.MaskToLayer(LayerMaskProvider.DeadPlayer);

            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Enemy))
            {
                float enemyDirectionX = transform.position.x - other.transform.position.x;
                float deathKickDirectionX = enemyDirectionX > 0 ? 1 : -1;
                RB.linearVelocity = new Vector2(deathKickDirectionX * _deathKick.x, _deathKick.y);
            }
            else
            {
                RB.gravityScale = 0.2f;
                RB.linearDamping = 5f;
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
        }
    }
}
