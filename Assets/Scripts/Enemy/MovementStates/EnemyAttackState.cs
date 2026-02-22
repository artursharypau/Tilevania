using UnityEngine;

namespace Enemy.MovementStates
{
    public class EnemyAttackState : EnemyState
    {
        private static readonly int Attacking = Animator.StringToHash("Attacking");

        private float _nextAttackTime;

        public EnemyAttackState(EnemyStateMachine fsm, EnemyController enemy)
            : base(fsm, enemy)
        {
        }

        public override void Enter()
        {
            Attack();
        }

        public override void Update()
        {
            if (!Enemy.ShouldAttack)
            {
                FSM.ChangeState(Enemy.RunState);
                return;
            }

            if (Time.time >= _nextAttackTime)
            {
                Attack();
            }
        }

        private void Attack()
        {
            Enemy.Anim.Play(Attacking);
            Enemy.CurrentTarget.TakeDamage();
            _nextAttackTime = Time.time + Enemy.AttackCooldown;
        }
    }
}
