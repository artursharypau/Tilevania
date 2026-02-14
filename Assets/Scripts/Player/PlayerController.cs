using Player.MovementStates;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _climbSpeed = 5f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private ushort _availableJumpCount = 2;
        [SerializeField] private float _coyoteJumpTime = 0.2f;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.1f;

        private float _lastGroundedTime;
        private LayerMask _groundLayerMask;
        private LayerMask _climbingLayerMask;

        public float MoveSpeed => _moveSpeed;
        public float ClimbSpeed => _climbSpeed;
        public float JumpForce => _jumpForce;
        public ushort AvailableJumpCount => _availableJumpCount;

        public bool CanClimb { get; private set; }
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
            _groundLayerMask = LayerMask.GetMask("Ground");
            _climbingLayerMask = LayerMask.GetMask("Climbing");

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

            if (IsGrounded())
            {
                _lastGroundedTime = Time.time;
            }
        }

        private void FixedUpdate()
        {
            MovementFSM.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((_climbingLayerMask & (1 << other.gameObject.layer)) != 0)
            {
                CanClimb = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if ((_climbingLayerMask & (1 << other.gameObject.layer)) != 0)
            {
                CanClimb = false;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
        }

        public bool IsGrounded()
        {
            return Mathf.Abs(RB.linearVelocity.y) > Mathf.Epsilon
                ? false
                : Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayerMask);
        }

        public bool CanJumpCoyote()
        {
            return Time.time < _lastGroundedTime + _coyoteJumpTime;
        }
    }
}
