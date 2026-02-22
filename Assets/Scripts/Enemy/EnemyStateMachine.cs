namespace Enemy
{
    public class EnemyStateMachine
    {
        public EnemyState Current { get; private set; }

        public void Initialize(EnemyState baseState)
        {
            Current = baseState;
            Current.Enter();
        }

        public void ChangeState(EnemyState newState)
        {
            if (Current == newState)
            {
                return;
            }

            Current.Exit();
            Current = newState;
            Current.Enter();
        }

        public void Update()
        {
            Current.Update();
        }

        public void FixedUpdate()
        {
            Current.FixedUpdate();
        }
    }
}
