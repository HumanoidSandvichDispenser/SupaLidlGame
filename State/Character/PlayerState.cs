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
        Vector2 joystick = Godot.Input.GetVector("look_left", "look_right",
                                                 "look_up", "look_down");

        if (Character.Inventory.SelectedItem is Items.Weapon weapon)
        {
            var isAttack1On = Godot.Input.IsActionPressed("attack1");
            var isAttack2On = Godot.Input.IsActionPressed("attack2");

            if (!weapon.ShouldHideIdle || isAttack1On)
            {
                var inputMethod = Utils.World.Instance.GlobalState
                    .Settings.InputMethod;
                switch (inputMethod)
                {
                    case Global.InputMethod.Joystick:
                        if (joystick.IsZeroApprox())
                        {
                            Character.Target = Character.Direction;
                        }
                        else
                        {
                            Character.Target = joystick;
                        }
                        break;
                    default:
                        Character.Target = dirToMouse;
                        break;
                }
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
