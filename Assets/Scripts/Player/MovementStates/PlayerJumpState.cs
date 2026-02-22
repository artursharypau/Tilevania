using UnityEngine;

namespace Player.MovementStates
{
    public class PlayerJumpState : PlayerState
    {
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");

        private readonly SpriteFlipper _spriteFlipper;

        private ushort _jumpCount;

        public PlayerJumpState(PlayerStateMachine fsm, PlayerController player, SpriteFlipper spriteFlipper)
            : base(fsm, player)
        {
            _spriteFlipper = spriteFlipper;
        }

        public override void Enter()
        {
            Jump();
            Player.Anim.SetBool(IsRunning, true);
        }

        public override void Update()
        {
            if (_jumpCount < Player.AvailableJumpCount && Player.Input.IsJumpActive())
            {
                Jump();
            }

            if (Player.IsGrounded())
            {
                if (Mathf.Abs(Player.Input.MoveInput.x) > Mathf.Epsilon)
                {
                    FSM.ChangeState(Player.RunState);
                }
                else
                {
                    FSM.ChangeState(Player.IdleState);
                }
            }
            else if (Player.CanClimb())
            {
                float moveY = Player.Input.MoveInput.y;
                bool isGrounded = Player.IsGrounded();
                if (Mathf.Abs(moveY) > Mathf.Epsilon && (!isGrounded || moveY > Mathf.Epsilon))
                {
                    FSM.ChangeState(Player.ClimbState);
                }
            }

            _spriteFlipper.CheckFlip(Player.Input.MoveInput.x, Player.transform);
        }

        public override void FixedUpdate()
        {
            Player.RB.linearVelocityX = Player.Input.MoveInput.x * Player.RunSpeed;
        }

        public override void Exit()
        {
            _jumpCount = 0;
            Player.Input.ConsumeJump();
            Player.Anim.SetBool(IsRunning, false);
        }

        private void Jump()
        {
            Player.Input.ConsumeJump();
            Player.RB.linearVelocity = new Vector2(Player.RB.linearVelocity.x, Player.JumpForce);
            ++_jumpCount;
        }
    }
}
