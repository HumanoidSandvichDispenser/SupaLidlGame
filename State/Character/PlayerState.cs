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
            else if (@event.IsActionPressed("equip_3"))
            {
                inventory.SelectedItem = inventory.GetItemByMap("equip_3");
            }

            if (@event.IsActionPressed("interact"))
            {
                // if looking at a trigger then interact with it
                player.InteractionRay.Trigger?.InvokeInteraction();
            }
        }

        return base.Input(@event);
    }

    public override CharacterState Process(double delta)
    {
        Character.Direction = Godot.Input.GetVector("left", "right",
                                                    "up", "down");
        Character.LookTowardsDirection();

        Vector2 mousePos = Character.GetGlobalMousePosition();
        Vector2 dirToMouse = Character.GlobalPosition.DirectionTo(mousePos);

        if (Character.Inventory.SelectedItem is Items.Weapon weapon)
        {
            var isAttack1On = Godot.Input.IsActionPressed("attack1");
            var isAttack2On = Godot.Input.IsActionPressed("attack2");

            if (!weapon.ShouldHideIdle || isAttack1On)
            {
                Character.Target = dirToMouse;
            }

            if (isAttack1On)
            {
                Character.UseCurrentItem();
            }
            else if (isAttack2On)
            {
                Character.UseCurrentItemAlt();
            }

        }

        return base.Process(delta);
    }
}
