using Godot;

namespace SupaLidlGame.Characters.State
{
    public partial class PlayerIdleState : PlayerState
    {
        [Export]
        public CharacterState MoveState { get; set; }

        public override CharacterState Enter(CharacterState previousState)
        {
            GD.Print("Entered idle state");
            if (previousState is not PlayerMoveState)
            {
                if (Character.Direction.LengthSquared() > 0)
                {
                    // other states like attacking or rolling can just delegate
                    // the work of checking if the player is moving to this idle
                    // state, so they do not have to manually check if the player
                    // wants to move to switch to the move state.
                    return MoveState;
                }
            }
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
