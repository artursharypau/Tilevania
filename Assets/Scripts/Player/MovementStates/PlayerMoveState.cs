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
                var moveY = Player.Input.MoveInput.y;
                var isGrounded = Player.IsGrounded();
                if (Mathf.Abs(moveY) > Mathf.Epsilon && (!isGrounded || moveY > Mathf.Epsilon))
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
