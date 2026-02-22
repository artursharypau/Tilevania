namespace Enemy
{
    public class EnemyState
    {
        protected EnemyStateMachine FSM { get; private set; }
        protected EnemyController Enemy { get; private set; }

        protected EnemyState(EnemyStateMachine fsm, EnemyController enemy)
        {
            FSM = fsm;
            Enemy = enemy;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void Exit()
        {
        }
    }
}
