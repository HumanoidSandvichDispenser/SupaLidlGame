using Godot;
using SupaLidlGame.Items;

namespace SupaLidlGame.Characters.State
{
    public partial class PlayerMoveState : PlayerState
    {
        [Export]
        public PlayerRollState RollState { get; set; }

        public override CharacterState Enter(CharacterState previousState)
        {
            Godot.GD.Print("Started moving");
            _player.Animation = "move";
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
                if (!_player.IsUsingAnyWeapon())
                {
                    return RollState;
                }
            }
            return base.Input(@event);
        }
    }
}
