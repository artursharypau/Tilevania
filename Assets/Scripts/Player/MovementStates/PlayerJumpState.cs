using UnityEngine;

namespace Player.MovementStates
{
    public class PlayerJumpState : PlayerState
    {
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

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
            Player.Anim.SetBool(IsMoving, true);
        }

        public override void Update()
        {
            if (_jumpCount < Player.AvailableJumpCount && Player.Input.IsJumpActive())
            {
                Jump();
            }

            if (Player.IsGrounded())
            {
                if (Mathf.Abs(Player.Input.MoveInput.x) < Mathf.Epsilon)
                {
                    FSM.ChangeState(Player.IdleState);
                }
                else
                {
                    FSM.ChangeState(Player.MoveState);
                }
            }
            else if (Player.CanClimb && Mathf.Abs(Player.Input.MoveInput.y) > Mathf.Epsilon)
            {
                if ((Player.IsGrounded() && Player.Input.MoveInput.y > Mathf.Epsilon) || !Player.IsGrounded())
                {
                    FSM.ChangeState(Player.ClimbState);
                }
            }

            _spriteFlipper.CheckFlip(Player.Input.MoveInput.x, Player.transform);
        }

        public override void FixedUpdate()
        {
            Player.RB.linearVelocityX = Player.Input.MoveInput.x * Player.MoveSpeed;
        }

        public override void Exit()
        {
            _jumpCount = 0;
            Player.Input.ConsumeJump();
            Player.Anim.SetBool(IsMoving, false);
        }

        private void Jump()
        {
            Player.Input.ConsumeJump();
            Player.RB.linearVelocity = new Vector2(Player.RB.linearVelocity.x, Player.JumpForce);
            ++_jumpCount;
        }
    }
}
