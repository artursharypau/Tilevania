using Player.MovementStates;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;

        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private ushort _availableJumpCount = 2;
        [SerializeField] private float _coyoteJumpTime = 0.2f;

        [SerializeField] private float _climbSpeed = 5f;
        [SerializeField] private float _climbBlockTime = 1f;

        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = Mathf.Epsilon;

        private bool _canClimb;
        private float _lastClimbTime;
        private LayerMask _climbingLayerMask;

        private float _lastGroundedTime;
        private LayerMask _groundLayerMask;

        public float MoveSpeed => _moveSpeed;
        public float ClimbSpeed => _climbSpeed;
        public float JumpForce => _jumpForce;
        public ushort AvailableJumpCount => _availableJumpCount;

        public Rigidbody2D RB { get; private set; }
        public Animator Anim { get; private set; }
        public PlayerInputHandler Input { get; private set; }

        public PlayerStateMachine MovementFSM { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerClimbState ClimbState { get; private set; }

        private void Awake()
        {
            _climbingLayerMask = LayerMask.GetMask("Climbing");
            _groundLayerMask = LayerMask.GetMask("Ground");

            RB = GetComponent<Rigidbody2D>();
            Anim = GetComponent<Animator>();
            Input = GetComponent<PlayerInputHandler>();

            var spriteFlipper = new SpriteFlipper(transform.localScale.x);

            MovementFSM = new PlayerStateMachine();
            IdleState = new PlayerIdleState(MovementFSM, this);
            MoveState = new PlayerMoveState(MovementFSM, this, spriteFlipper);
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
                _canClimb = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if ((_climbingLayerMask & (1 << other.gameObject.layer)) != 0)
            {
                _canClimb = false;
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
            return _canClimb && Time.time > _lastClimbTime + _climbBlockTime;
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
