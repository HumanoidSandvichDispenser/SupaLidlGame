using Godot;
using SupaLidlGame.Items;

namespace SupaLidlGame.Characters.State
{
    public partial class PlayerState : CharacterState
    {
        //public PlayerMachine PlayerMachine => Machine as PlayerMachine;
        protected Player _player => Character as Player;

        [Export]
        public PlayerIdleState IdleState { get; set; }

        public override CharacterState Input(InputEvent @event)
        {
            var inventory = Character.Inventory;

            #if DEBUG
            //if (@event.IsActionPressed("equip"))
            //{
            //    inventory.SelectedItem = inventory.GetNode<Items.Item>("Sword");
            //}
            #endif

            if (this is PlayerIdleState or PlayerMoveState
                    && !_player.Inventory.IsUsingItem)
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
            }

            return base.Input(@event);
        }

        public override CharacterState Process(double delta)
        {
            Character.Direction = Godot.Input.GetVector("ui_left", "ui_right",
                                                        "ui_up", "ui_down");
            Vector2 mousePos = Character.GetGlobalMousePosition();
            Vector2 dirToMouse = Character.GlobalPosition.DirectionTo(mousePos);
            if (Character.Inventory.SelectedItem is Weapon weapon)
            {
                if (!weapon.IsUsing)
                {
                    Character.Target = dirToMouse;
                }
            }
            else if (!Character.Direction.IsZeroApprox())
            {
                Character.Target = Character.Direction;
            }

            if (Godot.Input.IsActionPressed("attack1"))
            {
                if (Character.Inventory.SelectedItem is not null)
                {
                    Character.UseCurrentItem();
                }
            }

            return base.Process(delta);
        }
    }
}
