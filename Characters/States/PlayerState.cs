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
            /*
            if (@event is InputEventKey inputEventKey)
            {
                GD.Print("hello");
                if (inputEventKey.Keycode == Key.G)
                {
                    GD.Print("hi");
                }
                }*/
            if (@event.IsActionPressed("equip"))
            {
                Character.Inventory.SelectedItem = Character.Inventory.GetNode<Items.Item>("Sword");
                //Character.Inventory.AddItem();
            }

            if (@event.IsActionPressed("attack1"))
            {
                if (Character.Inventory.SelectedItem is not null)
                {
                    Character.UseCurrentItem();
                    //Character.Inventory.SelectedItem.Use();
                    //return AttackState;
                }
            }

            //if (this is PlayerAttackState)
            //{
            //    if (@event.IsActionReleased("attack1"))
            //    {
            //        return IdleState;
            //    }
            //}

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

            return base.Process(delta);
        }
    }
}
