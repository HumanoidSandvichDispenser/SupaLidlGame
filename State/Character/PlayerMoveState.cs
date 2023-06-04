using Godot;

namespace SupaLidlGame.State.Character;

public partial class PlayerMoveState : PlayerState
{
    [Export]
    public PlayerRollState RollState { get; set; }

    public override IState<CharacterState> Enter(IState<CharacterState> previousState)
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
            if (Character.Inventory.SelectedItem is Items.Weapon weapon)
            {
                if (!weapon.IsUsing)
                {
                    return RollState;
                }
            }
            else
            {
                return RollState;
            }
        }
        return base.Input(@event);
    }
}
