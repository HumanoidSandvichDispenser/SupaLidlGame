using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.State.Character;

public abstract partial class PlayerState : CharacterState
{
    protected Player _player => Character as Player;

    [Export]
    public PlayerIdleState IdleState { get; set; }

    public override CharacterState Input(InputEvent @event)
    {
        var inventory = Character.Inventory;
        var player = _player;

        if (this is PlayerIdleState or PlayerMoveState &&
            !player.Inventory.IsUsingItem)
        {
            if (@event.IsActionPressed("equip_1"))
            {
                inventory.SelectedItem = inventory.GetItemByMap("equip_1");
            }
            else if (@event.IsActionPressed("equip_2"))
            {
                inventory.SelectedItem = inventory.GetItemByMap("equip_2");
            }

            if (@event.IsActionPressed("interact"))
            {
                // if looking at a trigger then interact with it
                GD.Print("interacting");
                player.InteractionRay.Trigger?.InvokeInteraction();
            }
        }

        return base.Input(@event);
    }

    public override CharacterState Process(double delta)
    {
        Character.Direction = Godot.Input.GetVector("ui_left", "ui_right",
                                                    "ui_up", "ui_down");
        Character.LookTowardsDirection();

        Vector2 mousePos = Character.GetGlobalMousePosition();
        Vector2 dirToMouse = Character.GlobalPosition.DirectionTo(mousePos);

        bool targetTowards(Items.Item item)
        {
            if (Character.Inventory.SelectedItem is Items.Weapon weapon)
            {
                var isPressed = Godot.Input.IsActionPressed("attack1");
                if (!weapon.IsUsing)
                {
                    var ret = false;

                    if (!weapon.ShouldHideIdle || isPressed)
                    {
                        Character.Target = dirToMouse;
                        ret = true;
                    }

                    if (isPressed)
                    {
                        Character.UseCurrentItem();
                    }

                    return ret;
                }
                else
                {
                    if (!isPressed)
                    {
                        Character.DeuseCurrentItem();
                    }
                }
            }
            return false;
        }

        var item = Character.Inventory.SelectedItem;
        var offhand = Character.Inventory.OffhandItem;

        var _ = targetTowards(item) || targetTowards(offhand);

        return base.Process(delta);
    }
}
