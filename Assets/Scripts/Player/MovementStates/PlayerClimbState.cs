using UnityEngine;

namespace Player.MovementStates
{
    public class PlayerClimbState : PlayerState
    {
        private static readonly int IsClimbing = Animator.StringToHash("IsClimbing");
        private static readonly int ClimbAnimSpeed = Animator.StringToHash("ClimbSpeed");

        private float _gravityScale;
        private float _climbAnimSpeed;

        public PlayerClimbState(PlayerStateMachine fsm, PlayerController player)
            : base(fsm, player)
        {
        }

        public override void Enter()
        {
            _gravityScale = Player.RB.gravityScale;
            _climbAnimSpeed = Player.Anim.GetFloat(ClimbAnimSpeed);
            Player.RB.gravityScale = 0;
            Player.Anim.SetBool(IsClimbing, true);
        }

        public override void Update()
        {
            Player.Anim.SetFloat(ClimbAnimSpeed, Mathf.Abs(Player.Input.MoveInput.y) > Mathf.Epsilon ? _climbAnimSpeed : 0f);

            if (Player.Input.IsJumpActive())
            {
                FSM.ChangeState(Player.JumpState);
                Player.BlockClimbTemporary();
            }
            else if (!Player.CanClimb() || Player.IsGrounded())
            {
                if (Mathf.Abs(Player.Input.MoveInput.x) > Mathf.Epsilon)
                {
                    FSM.ChangeState(Player.MoveState);
                }
                else
                {
                    FSM.ChangeState(Player.IdleState);
                }
            }
        }

        public override void FixedUpdate()
        {
            Player.RB.linearVelocity = new Vector2(0f, Player.Input.MoveInput.y * Player.ClimbSpeed);
        }

        public override void Exit()
        {
            Player.RB.gravityScale = _gravityScale;
            Player.Anim.SetBool(IsClimbing, false);
            Player.Anim.SetFloat(ClimbAnimSpeed, _climbAnimSpeed);
        }
    }
}
