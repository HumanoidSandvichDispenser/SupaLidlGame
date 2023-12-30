using Godot;

namespace SupaLidlGame.State.Character;

public abstract partial class CharacterState : Node, IState<CharacterState>
{
    [Export]
    public Characters.Character Character { get; set; }

    public bool CanEnterWhileUsingItem { get; set; }

    public virtual IState<CharacterState> Enter(IState<CharacterState> prev) => null;

    public virtual void Exit(IState<CharacterState> next)
    {

    }

    public virtual CharacterState Process(double delta)
    {
        var item = Character.Inventory.SelectedItem;

        bool targetTowards(Items.Item item)
        {
            if (item is Items.Weapon weapon)
            {
                if (weapon.IsUsing)
                {
                    if (weapon.ShouldFreezeAngleOnUse)
                    {
                        Character.Target = weapon.UseDirection;
                    }
                    return true;
                }
            }
            return false;
        }

        targetTowards(item);

        return null;
    }

    public virtual CharacterState PhysicsProcess(double delta)
    {
        if (!Character.IsAlive)
        {
            Character.Velocity = Vector2.Zero;
            Character.MoveAndSlide();
            return null;
        }

        Character.Velocity = Character.NetImpulse;

        if (Character.NetImpulse.LengthSquared() < Mathf.Pow(Character.Speed, 2))
        {
            Character.Velocity += Character.Direction.Normalized()
                * Character.Speed;
            // we should only modify velocity that is in the player's control
            Character.ModifyVelocity();
        }

        Character.NetImpulse = Character.NetImpulse.MoveToward(
            Vector2.Zero,
            (float)delta * Character.Speed * Character.Friction);

        Character.MoveAndSlide();

        return null;
    }

    public virtual CharacterState Input(InputEvent @event) => null;
}
