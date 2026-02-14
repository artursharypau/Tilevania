namespace Player
{
    public abstract class PlayerState
    {
        protected PlayerStateMachine FSM { get; private set; }
        protected PlayerController Player { get; private set; }

        protected PlayerState(PlayerStateMachine fsm, PlayerController player)
        {
            FSM = fsm;
            Player = player;
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