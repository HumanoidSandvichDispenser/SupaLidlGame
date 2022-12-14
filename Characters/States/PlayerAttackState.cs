using Godot;
using SupaLidlGame.Items;

namespace SupaLidlGame.Characters.State
{
    public partial class PlayerAttackState : PlayerState
    {
        private double _attackTime = 0;

        public override CharacterState Enter(CharacterState previousState)
        {
            if (Character.Inventory.SelectedItem is Weapon weapon)
            {
                _attackTime = weapon.UseTime;
                weapon.Visible = true;
                Character.Inventory.SelectedItem.Use();
            }
            else
            {
                return IdleState;
            }
            return base.Enter(previousState);
        }

        public override void Exit(CharacterState nextState)
        {
            if (Character.Inventory.SelectedItem is null)
            {

            }
            Character.Inventory.SelectedItem.Deuse();
            if (Character.Inventory.SelectedItem is Weapon weapon)
            {
                //weapon.Visible = false;
            }
            base.Exit(nextState);
        }

        public override CharacterState Input(InputEvent @event)
        {
            return base.Input(@event);
        }

        public override CharacterState PhysicsProcess(double delta)
        {
            Character.Velocity *= 0.5f;
            return base.PhysicsProcess(delta);
        }

        public override CharacterState Process(double delta)
        {
            if ((_attackTime -= delta) <= 0)
            {
                return IdleState;
            }
            return base.Process(delta);
        }
    }
}
