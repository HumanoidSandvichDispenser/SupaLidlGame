using Godot;
using SupaLidlGame.Items;

namespace SupaLidlGame.Characters
{
    public partial class Player : Character
    {
        private AnimatedSprite2D _sprite;
        private string _spriteAnim;

        public string Animation
        {
            get => _sprite.Animation;
            set
            {
                // the Player.Animation property might be set before this node is
                // even ready, so if _sprite is null, then we just hold the new
                // animation in a temp value, which will be assigned once this
                // node is ready
                if (_sprite is null)
                {
                    _spriteAnim = value;
                    return;
                }

                _sprite.Animation = value;
            }
        }

        public override void _Ready()
        {
            _sprite = GetNode<AnimatedSprite2D>("Sprite");
            if (_spriteAnim != default)
            {
                _sprite.Animation = _spriteAnim;
            }
        }

        public override void ModifyVelocity()
        {
            if (StateMachine.State is State.PlayerRollState)
            {
                Velocity *= 2;
            }

            if (Inventory.SelectedItem is Weapon weapon)
            {
                /*if (weapon is Sword sword)
                {
                    if (sword.IsAttacking)
                    {
                        Velocity *= 0.5f;
                    }
                }
                else*/
                if (weapon.IsUsing)
                {
                    Velocity *= 0.75f;
                }
            }
        }
    }
}
