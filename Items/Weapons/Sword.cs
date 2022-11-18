using Godot;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items.Weapons
{
    public partial class Sword : Weapon
    {
        [Export]
        public Hitbox Hitbox { get; set; }

        public override void Equip(Character character)
        {
            base.Equip(character);
        }

        public override void Use()
        {
            Hitbox.IsEnabled = true;
        }

        public override void Deuse()
        {
            Hitbox.IsEnabled = false;
        }
    }
}
