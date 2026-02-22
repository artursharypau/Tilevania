using Player.MovementStates;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _runSpeed = 10f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private ushort _availableJumpCount = 2;
        [SerializeField] private float _coyoteJumpTime = 0.2f;
        [SerializeField] private float _climbSpeed = 5f;
        [SerializeField] private float _climbBlockTime = 1f;
        [SerializeField] private Tilemap _climbingTilemap;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = Mathf.Epsilon;

        private float _lastClimbTime;
        private LayerMask _climbingLayerMask;
        private float _lastGroundedTime;
        private LayerMask _groundLayerMask;
        private HealthController _healthController;

        public float RunSpeed => _runSpeed;
        public float ClimbSpeed => _climbSpeed;
        public float JumpForce => _jumpForce;
        public ushort AvailableJumpCount => _availableJumpCount;
        public Vector2 CurrentClimbPosition { get; private set; } = Vector3.zero;

        public Rigidbody2D RB { get; private set; }
        public Animator Anim { get; private set; }
        public PlayerInputHandler Input { get; private set; }

        public PlayerStateMachine MovementFSM { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerRunState RunState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerClimbState ClimbState { get; private set; }

        private void Awake()
        {
            _climbingLayerMask = LayerMask.GetMask("Climbing");
            _groundLayerMask = LayerMask.GetMask("Ground", "Bouncing");
            _healthController = GetComponent<HealthController>();

            RB = GetComponent<Rigidbody2D>();
            Anim = GetComponent<Animator>();
            Input = GetComponent<PlayerInputHandler>();

            SpriteFlipper spriteFlipper = new(transform.localScale.x);

            MovementFSM = new PlayerStateMachine();
            IdleState = new PlayerIdleState(MovementFSM, this);
            RunState = new PlayerRunState(MovementFSM, this, spriteFlipper);
            JumpState = new PlayerJumpState(MovementFSM, this, spriteFlipper);
            ClimbState = new PlayerClimbState(MovementFSM, this);

            MovementFSM.Initialize(IdleState);
        }

        private void Update()
        {
            MovementFSM.Update();
        }

        private void FixedUpdate()
        {
            MovementFSM.FixedUpdate();

            if (IsGrounded())
            {
                _lastGroundedTime = Time.time;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((_climbingLayerMask & (1 << other.gameObject.layer)) != 0)
            {
                Vector3Int cellPosition = _climbingTilemap.WorldToCell(transform.position);
                CurrentClimbPosition = _climbingTilemap.GetCellCenterWorld(cellPosition);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if ((_climbingLayerMask & (1 << other.gameObject.layer)) != 0)
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
                : Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayerMask);
        }
    }
}
