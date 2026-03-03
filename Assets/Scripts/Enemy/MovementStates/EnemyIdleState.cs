using Common;
using UnityEngine;

namespace Enemy.MovementStates
{
    public class EnemyIdleState : EnemyState
    {
        private readonly SpriteFlipper _spriteFlipper;

        private float _timer;

        public EnemyIdleState(EnemyStateMachine fsm, EnemyController enemy, SpriteFlipper spriteFlipper)
            : base(fsm, enemy)
        {
            _spriteFlipper = spriteFlipper;
        }

        public override void Enter()
        {
            _timer = Enemy.IdleTime;

            Enemy.RB.linearVelocityX = 0f;
        }

        public override void Update()
        {
            if (Enemy.ShouldAttack)
            {
                FSM.ChangeState(Enemy.AttackState);
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _spriteFlipper.CheckFlip(-Enemy.transform.localScale.x, Enemy.transform);
                FSM.ChangeState(Enemy.RunState);
            }
        }
    }
}