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
            #if DEBUG
            if (@event.IsActionPressed("equip"))
            {
                //Character.Inventory.SelectedItem = Character.Inventory.GetNode<Items.Item>("Sword");
                //Character.Inventory.SelectedItem = Character.
                Character.Inventory.SelectFirstItem();
            }
            #endif

            return base.Input(@event);
        }

        public override CharacterState Process(double delta)
        {
            Character.Direction = Godot.Input.GetVector("ui_left", "ui_right",
                                                        "ui_up", "ui_down");
            Vector2 mousePos = Character.GetGlobalMousePosition();
            Vector2 dirToMouse = Character.GlobalPosition.DirectionTo(mousePos);
            if (Character.Inventory.PrimaryItem is Weapon weapon)
            {
                if (!weapon.IsUsing)
                {
                    Character.Target = dirToMouse;
                }
            }

            if (Godot.Input.IsActionPressed("attack1"))
            {
                if (Character.Inventory.PrimaryItem is not null)
                {
                    Character.UseCurrentItem();
                }
            }

            return base.Process(delta);
        }
    }
}
