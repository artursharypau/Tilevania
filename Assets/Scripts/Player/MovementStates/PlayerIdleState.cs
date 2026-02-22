using UnityEngine;

namespace Player.MovementStates
{
    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(PlayerStateMachine fsm, PlayerController player)
            : base(fsm, player)
        {
        }

        public override void Enter()
        {
            Player.RB.linearVelocity = new Vector2(0f, Player.RB.linearVelocity.y);
        }

        public override void Update()
        {
            if (Player.Input.IsJumpActive() && Player.CanJumpCoyote())
            {
                FSM.ChangeState(Player.JumpState);
            }
            else if (Mathf.Abs(Player.Input.MoveInput.x) > Mathf.Epsilon)
            {
                FSM.ChangeState(Player.RunState);
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
        }
    }
}
