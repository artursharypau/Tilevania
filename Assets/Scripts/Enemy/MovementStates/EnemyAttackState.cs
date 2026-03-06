using Common;
using UnityEngine;

namespace Enemy.MovementStates
{
    public class EnemyAttackState : EnemyState
    {
        private static readonly int Attacking = Animator.StringToHash("Attacking");

        private readonly SpriteFlipper _spriteFlipper;

        private float _nextAttackTime;

        public EnemyAttackState(EnemyStateMachine fsm, EnemyController enemy, SpriteFlipper spriteFlipper)
            : base(fsm, enemy)
        {
            _spriteFlipper = spriteFlipper;
        }

        public override void Enter()
        {
            float playerDirectionX = Enemy.CurrentTarget.transform.position.x - Enemy.transform.position.x;
            if ((Enemy.IsFacingRight() && playerDirectionX < 0)
                || (!Enemy.IsFacingRight() && playerDirectionX > 0))
            {
                _spriteFlipper.CheckFlip(-Enemy.transform.localScale.x, Enemy.transform);
            }
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
            Enemy.Anim.SetTrigger(Attacking);
            Enemy.CurrentTarget.TakeDamage(1, Enemy.gameObject);
            _nextAttackTime = Time.time + Enemy.AttackCooldown;
        }
    }
}
