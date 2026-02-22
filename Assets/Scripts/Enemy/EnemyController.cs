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

        private LayerMask _playerLayerMask;

        public float IdleTime => _idleTime;
        public float RunSpeed => _runSpeed;
        public float RayDistance => _rayDistance;
        public Transform LedgeCheck => _ledgeCheck;
        public float AttackCooldown => _attackCooldown;

        public bool ShouldAttack { get; private set; }
        public LayerMask GroundLayerMask { get; private set; }
        public Rigidbody2D RB { get; private set; }
        public Animator Anim { get; private set; }

        public EnemyStateMachine MovementFsm { get; private set; }
        public EnemyIdleState IdleState { get; private set; }
        public EnemyRunState RunState { get; private set; }
        public EnemyAttackState AttackState { get; private set; }

        public HealthController CurrentTarget { get; private set; }

        private void Awake()
        {
            _playerLayerMask = LayerMask.GetMask("Player");

            ShouldAttack = false;
            GroundLayerMask = LayerMask.GetMask("Ground", "Bouncing");
            RB = GetComponent<Rigidbody2D>();
            Anim = GetComponent<Animator>();

            MovementFsm = new EnemyStateMachine();
            IdleState = new EnemyIdleState(MovementFsm, this, new SpriteFlipper(transform.localScale.x));
            RunState = new EnemyRunState(MovementFsm, this);
            AttackState = new EnemyAttackState(MovementFsm, this);
        }

        private void Start()
        {
            MovementFsm.Initialize(RunState);
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
            if ((_playerLayerMask & (1 << other.gameObject.layer)) != 0)
            {
                ShouldAttack = true;
                CurrentTarget = other.GetComponent<HealthController>();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if ((_playerLayerMask & (1 << other.gameObject.layer)) != 0)
            {
                ShouldAttack = false;
                CurrentTarget = null;
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
    }
}
