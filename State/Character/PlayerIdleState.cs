using Godot;

namespace SupaLidlGame.State.Character
{
    public partial class PlayerIdleState : PlayerState
    {
        [Export]
        public CharacterState MoveState { get; set; }

        public override IState<CharacterState> Enter(IState<CharacterState> previousState)
        {
            GD.Print("Entered idle state");
            if (previousState is not PlayerMoveState)
            {
                if (Character.Direction.LengthSquared() > 0.01f)
                {
                    // other states like attacking or rolling can just delegate
                    // the work of checking if the player is moving to this idle
                    // state, so they do not have to manually check if the player
                    // wants to move to switch to the move state.
                    return MoveState;
                }
            }
            _player.Animation = "idle";
            return base.Enter(previousState);
        }

        public override CharacterState Process(double delta)
        {
            base.Process(delta);
            if (Character.Direction.LengthSquared() > 0)
            {
                return MoveState;
            }
            return null;
        }
    }
}
