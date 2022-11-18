using Godot;

namespace SupaLidlGame.Characters.State
{
    public partial class PlayerMoveState : PlayerState
    {
        [Export]
        public CharacterState IdleState { get; set; }

        [Export]
        public CharacterState RollState { get; set; }

        public override CharacterState Enter(CharacterState previousState)
        {
            Godot.GD.Print("Started moving");
            return base.Enter(previousState);
        }

        public override CharacterState Process(double delta)
        {
            base.Process(delta);
            if (Character.Direction.LengthSquared() == 0)
            {
                return IdleState;
            }
            return null;
        }

        public override CharacterState Input(InputEvent @event)
        {
            if (@event.IsActionPressed("roll"))
            {
                return RollState;
            }
            return null;
        }
    }
}
