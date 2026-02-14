using UnityEngine;

namespace Player.MovementStates
{
    public class PlayerMoveState : PlayerState
    {
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        private readonly SpriteFlipper _spriteFlipper;

        public PlayerMoveState(PlayerStateMachine fsm, PlayerController player, SpriteFlipper spriteFlipper)
            : base(fsm, player)
        {
            _spriteFlipper = spriteFlipper;
        }

        public override void Enter()
        {
            Player.Anim.SetBool(IsMoving, true);
        }

        public override void Update()
        {
            if (Mathf.Abs(Player.Input.MoveInput.x) < Mathf.Epsilon)
            {
                FSM.ChangeState(Player.IdleState);
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

            _spriteFlipper.CheckFlip(Player.Input.MoveInput.x, Player.transform);
        }

        public override void FixedUpdate()
        {
            Player.RB.linearVelocityX = Player.Input.MoveInput.x * Player.MoveSpeed;
        }

        public override void Exit()
        {
            Player.Anim.SetBool(IsMoving, false);
        }
    }
}
