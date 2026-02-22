using UnityEngine;

namespace Player.MovementStates
{
    public class PlayerRunState : PlayerState
    {
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");

        private readonly SpriteFlipper _spriteFlipper;

        public PlayerRunState(PlayerStateMachine fsm, PlayerController player, SpriteFlipper spriteFlipper)
            : base(fsm, player)
        {
            _spriteFlipper = spriteFlipper;
        }

        public override void Enter()
        {
            Player.Anim.SetBool(IsRunning, true);
        }

        public override void Update()
        {
            if (Player.Input.IsJumpActive() && Player.CanJumpCoyote())
            {
                FSM.ChangeState(Player.JumpState);
            }
            else if (Mathf.Abs(Player.Input.MoveInput.x) < Mathf.Epsilon)
            {
                FSM.ChangeState(Player.IdleState);
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
            Player.Anim.SetBool(IsRunning, false);
        }
    }
}
