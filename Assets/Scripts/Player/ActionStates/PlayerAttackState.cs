namespace Player.ActionStates
{
    public class PlayerAttackState : PlayerState
    {
        public PlayerAttackState(PlayerStateMachine fsm, PlayerController player)
            : base(fsm, player)
        {
        }

        public override void Update()
        {
            if (Player.Input.IsAttackBufferEmpty())
            {
                FSM.ChangeState(Player.NoneState);
                return;
            }

            if (Player.BulletController.IsReady)
            {
                Player.BulletController.Fire();
                Player.Input.ConsumeAttack();
            }
        }
    }
}
