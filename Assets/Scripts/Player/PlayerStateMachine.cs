namespace Player
{
    public class PlayerStateMachine
    {
        public PlayerState Current { get; private set; }

        public void Initialize(PlayerState baseState)
        {
            Current = baseState;
        }

        public void ChangeState(PlayerState newState)
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