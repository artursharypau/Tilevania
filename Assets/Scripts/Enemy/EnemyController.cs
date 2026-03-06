using Common;
using Enemy.MovementStates;
using Player;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float _idleTime = 2f;
        [SerializeField] private float _runSpeed = 1f;
        [SerializeField] private float _rayDistance = 0.5f;
        [SerializeField] private Transform _ledgeCheck;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private ushort _healthPoints = 3;
        [SerializeField] private ParticleSystem _bloodParticles;

        public float IdleTime => _idleTime;
        public float RunSpeed => _runSpeed;
        public float RayDistance => _rayDistance;
        public Transform LedgeCheck => _ledgeCheck;
        public float AttackCooldown => _attackCooldown;

        public bool ShouldAttack { get; private set; }
        public Rigidbody2D RB { get; private set; }
        public Animator Anim { get; private set; }

        public EnemyStateMachine MovementFsm { get; private set; }
        public EnemyIdleState IdleState { get; private set; }
        public EnemyRunState RunState { get; private set; }
        public EnemyAttackState AttackState { get; private set; }

        public PlayerHealthController CurrentTarget { get; private set; }

        private void Awake()
        {
            ShouldAttack = false;
            RB = GetComponent<Rigidbody2D>();
            Anim = GetComponent<Animator>();

            SpriteFlipper spriteFlipper = new(transform.localScale.x);

            MovementFsm = new EnemyStateMachine();
            IdleState = new EnemyIdleState(MovementFsm, this, spriteFlipper);
            RunState = new EnemyRunState(MovementFsm, this);
            AttackState = new EnemyAttackState(MovementFsm, this, spriteFlipper);
        }

        private void Start()
        {
            MovementFsm.Initialize(IdleState);
        }

        private void Update()
        {
            MovementFsm.Update();
        }

        private void FixedUpdate()
        {
            MovementFsm.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Player))
            {
                PlayerLegs playerLegs = other.GetComponentInChildren<PlayerLegs>();
                if (playerLegs && playerLegs.IsOnTheLayer(LayerMaskProvider.Enemy))
                {
                    _bloodParticles.Play(true);
                    Die();
                }
                else
                {
                    ShouldAttack = true;
                    CurrentTarget = other.GetComponent<PlayerHealthController>();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Player, LayerMaskProvider.DeadPlayer))
            {
                ShouldAttack = false;
                CurrentTarget = null;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (LayerMaskProvider.Contains(other.gameObject.layer, LayerMaskProvider.Bullet))
            {
                --_healthPoints;
                _bloodParticles.Play(true);

                if (_healthPoints == 0)
                {
                    Die();
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_ledgeCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(_ledgeCheck.position, Vector2.down * _rayDistance);
                Gizmos.DrawRay(_ledgeCheck.position, IsFacingRight() ? Vector2.right : Vector2.left * _rayDistance);
            }
        }

        public bool IsFacingRight()
        {
            return transform.localScale.x > 0;
        }

        private void Die()
        {
            enabled = false;
            Destroy(gameObject, _bloodParticles.main.duration);
        }
    }
}
