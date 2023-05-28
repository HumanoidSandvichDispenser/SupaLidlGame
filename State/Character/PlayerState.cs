using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.State.Character
{
    public abstract partial class PlayerState : CharacterState
    {
        protected Player _player => Character as Player;

        [Export]
        public PlayerIdleState IdleState { get; set; }

        public override CharacterState Input(InputEvent @event)
        {
            var inventory = Character.Inventory;

            if (this is PlayerIdleState or PlayerMoveState &&
                    !_player.Inventory.IsUsingItem)
            {
                if (@event.IsActionPressed("equip_1"))
                {
                    inventory.SelectedItem = inventory.GetItemByMap("equip_1");
                }
            }

            return base.Input(@event);
        }

        public override CharacterState Process(double delta)
        {
            Character.Direction = Godot.Input.GetVector("ui_left", "ui_right",
                                                        "ui_up", "ui_down");
            Vector2 mousePos = Character.GetGlobalMousePosition();
            Vector2 dirToMouse = Character.GlobalPosition.DirectionTo(mousePos);

            bool faceToDir = !Character.Direction.IsZeroApprox();

            if (Character.Inventory.SelectedItem is Items.Weapon weapon)
            {
                if (!weapon.IsUsing)
                {
                    if (weapon.ShouldHideIdle)
                    {
                        faceToDir = true;
                    }
                    else
                    {
                        Character.Target = dirToMouse;
                    }
                }
                else
                {
                    faceToDir = false;
                }
            }

            if (faceToDir)
            {
                Character.Target = Character.Direction;
            }

            if (Godot.Input.IsActionPressed("attack1"))
            {
                var item = Character.Inventory.SelectedItem;
                if (item is not null)
                {
                    if (!item.IsUsing)
                    {
                        Character.Target = dirToMouse;
                    }
                    Character.UseCurrentItem();
                }
            }

            return base.Process(delta);
        }
    }
}
