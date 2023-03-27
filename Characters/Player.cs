using Godot;
using SupaLidlGame.Utils;

namespace SupaLidlGame.Characters
{
    public partial class Player : Character
    {
        private AnimatedSprite2D _sprite;
        private string _spriteAnim;

        [Export]
        public PlayerCamera Camera { get; set; }

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

                _sprite.Play(value);
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

            base.ModifyVelocity();
        }

        public override void Stun(float time)
        {
            base.Stun(time);
            Camera.Shake(2, 0.8f);
            // TODO: implement visual effects for stun
        }

        public override void Die()
        {
            GD.Print("died");
            //base.Die();
        }

        public bool IsUsingAnyWeapon()
        {
            bool checkItem(Items.Item item)
            {
                if (item is Items.Weapon weapon)
                {
                    return weapon.IsUsing;
                }
                return false;
            }
            return checkItem(Inventory.PrimaryItem) ||
                checkItem(Inventory.OffhandItem);
        }
    }
}
