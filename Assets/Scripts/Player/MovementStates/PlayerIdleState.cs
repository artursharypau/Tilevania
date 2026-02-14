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
            if (Mathf.Abs(Player.Input.MoveInput.x) > Mathf.Epsilon)
            {
                FSM.ChangeState(Player.MoveState);
            }
            else if (Player.CanJumpCoyote() && Player.Input.IsJumpActive())
            {
                FSM.ChangeState(Player.JumpState);
            }
            else if (Player.CanClimb && Mathf.Abs(Player.Input.MoveInput.y) > Mathf.Epsilon)
            {
                if ((Player.IsGrounded() && Player.Input.MoveInput.y > Mathf.Epsilon) || !Player.IsGrounded())
                {
                    FSM.ChangeState(Player.ClimbState);
                }
            }
        }
    }
}
