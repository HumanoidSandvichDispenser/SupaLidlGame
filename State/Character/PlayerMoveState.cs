using Godot;

namespace SupaLidlGame.State.Character;

public partial class PlayerMoveState : PlayerState
{
    [Export]
    public CharacterState AbilityState { get; set; }

    public double MoveDuration { get; private set; }

    public override IState<CharacterState> Enter(IState<CharacterState> previousState)
    {
        _player.MovementAnimation.Play("move");
        MoveDuration = 0;
        return base.Enter(previousState);
    }

    public override CharacterState Process(double delta)
    {
        base.Process(delta);
        MoveDuration += delta;
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
                bool canUseAbility = true;

                if (!AbilityState.CanEnterWhileUsingItem && weapon.IsUsing)
                {
                    canUseAbility = false;
                }

                if (canUseAbility)
                {
                    return AbilityState;
                }
            }
            else
            {
                return AbilityState;
            }
        }
        return base.Input(@event);
    }
}
