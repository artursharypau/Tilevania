using UnityEngine;

namespace Enemy.MovementStates
{
    public class EnemyRunState : EnemyState
    {
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");

        public EnemyRunState(EnemyStateMachine fsm, EnemyController enemy)
            : base(fsm, enemy)
        {
        }

        public override void Enter()
        {
            Enemy.Anim.SetBool(IsRunning, true);
        }

        public override void Update()
        {
            if (Enemy.ShouldAttack)
            {
                FSM.ChangeState(Enemy.AttackState);
                return;
            }

            RaycastHit2D groundInfo = Physics2D.Raycast(
                Enemy.LedgeCheck.position,
                Vector2.down,
                Enemy.RayDistance,
                Enemy.GroundLayerMask);
            RaycastHit2D wallInfo = Physics2D.Raycast(
                Enemy.LedgeCheck.position,
                Enemy.IsFacingRight() ? Vector2.right : Vector2.left,
                Enemy.RayDistance,
                Enemy.GroundLayerMask);

            if (groundInfo.collider == false || wallInfo.collider == true)
            {
                FSM.ChangeState(Enemy.IdleState);
            }
        }

        public override void FixedUpdate()
        {
            Enemy.RB.linearVelocityX = Enemy.transform.localScale.x * Enemy.RunSpeed;
        }

        public override void Exit()
        {
            Enemy.RB.linearVelocityX = 0f;
            Enemy.Anim.SetBool(IsRunning, false);
        }
    }
}
