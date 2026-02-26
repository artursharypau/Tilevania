namespace Player.ActionStates
{
    public class PlayerNoneState : PlayerState
    {
        public PlayerNoneState(PlayerStateMachine fsm, PlayerController player)
            : base(fsm, player)
        {
        }

        public override void Update()
        {
            if (!Player.Input.IsAttackBufferEmpty())
            {
                FSM.ChangeState(Player.AttackState);
            }
        }
    }
}
