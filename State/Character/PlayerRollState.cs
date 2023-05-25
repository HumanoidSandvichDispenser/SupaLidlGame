using Godot;

namespace SupaLidlGame.State.Character
{
    public partial class PlayerRollState : PlayerState
    {
        private double _timeLeftToRoll = 0;

        private Vector2 _rollDirection = Vector2.Zero;

        public override IState<CharacterState> Enter(IState<CharacterState> previousState)
        {
            _timeLeftToRoll = 0.5;
            // roll the direction we were previously moving in
            _rollDirection = Character.Direction;
            Character.Target = Character.Direction;
            return base.Enter(previousState);
        }

        public override void Exit(IState<CharacterState> nextState)
        {
            // we want to reset our state variables in case we are forced out of
            // this state (e.g. from death)
            _timeLeftToRoll = 0;
            _rollDirection = Character.Direction;
            base.Exit(nextState);
        }

        public override CharacterState Process(double delta)
        {
            Character.Direction = _rollDirection;
            if ((_timeLeftToRoll -= delta) <= 0)
            {
                return IdleState;
            }
            return null;
        }
    }
}
