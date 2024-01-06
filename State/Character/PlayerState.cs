using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.State.Character;

public abstract partial class PlayerState : CharacterState
{
    protected Player _player => Character as Player;

    [Export]
    public PlayerIdleState IdleState { get; set; }

    [Export]
    public PlayerMaxLevelState MaxLevelState { get; set; }

    public override CharacterState UnhandledInput(InputEvent @event)
    {
        var inventory = Character.Inventory;
        var player = _player;

        if (this is PlayerIdleState or PlayerMoveState &&
            !player.Inventory.IsUsingItem)
        {
            if (@event.IsActionPressed("equip_1"))
            {
                //inventory.SelectedItem = inventory.GetItemByMap("equip_1");
                inventory.SelectedIndex = 0;
            }
            else if (@event.IsActionPressed("equip_2"))
            {
                //inventory.SelectedItem = inventory.GetItemByMap("equip_2");
                inventory.SelectedIndex = 1;
            }
            else if (@event.IsActionPressed("equip_3"))
            {
                //inventory.SelectedItem = inventory.GetItemByMap("equip_3");
                inventory.SelectedIndex = 2;
            }
            else if (@event.IsActionPressed("next_item"))
            {
                inventory.EquipIndex(++inventory.CurrentQuickSwitchIndex);
            }
            else if (@event.IsActionPressed("prev_item"))
            {
                inventory.EquipIndex(--inventory.CurrentQuickSwitchIndex);
            }

            if (@event.IsActionPressed("interact"))
            {
                // if looking at a trigger then interact with it
                player.InteractionRay.Trigger?.InvokeInteraction();
            }

            if (@event.IsActionPressed("cast"))
            {
                if (_player.Stats.Level.Value >= MaxLevelState.LevelCost)
                {
                    return MaxLevelState;
                }
            }
        }

        return base.UnhandledInput(@event);
    }

    public override CharacterState Process(double delta)
    {
        Character.Direction = Godot.Input.GetVector("left", "right",
                                                    "up", "down");

        var player = _player;

        var desiredTarget = player.GetDesiredInputFromInput();
        if (!desiredTarget.IsZeroApprox())
        {
            // can never be zero
            player.DesiredTarget = desiredTarget;
        }

        Character.LookTowardsDirection();

        if (Character.Inventory.SelectedItem is Items.Weapon weapon)
        {
            var isAttack1On = Godot.Input.IsActionPressed("attack1");
            var isAttack2On = Godot.Input.IsActionPressed("attack2");

            if (!weapon.ShouldHideIdle || !weapon.ShouldFreezeAngleOnUse || isAttack1On)
            {
                player.Target = player.DesiredTarget;
            }

            if (isAttack1On)
            {
                Character.UseCurrentItem();
            }
            else if (isAttack2On)
            {
                Character.UseCurrentItemAlt();
            }

            if (Godot.Input.IsActionJustReleased("attack1"))
            {
                Character.DeuseCurrentItem();
            }
        }

        return base.Process(delta);
    }
}
