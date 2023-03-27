using Godot;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items
{
    public abstract partial class Weapon : Item
    {
        public double RemainingUseTime { get; protected set; } = 0;

        public bool IsUsing => RemainingUseTime > 0;

        public new WeaponInfo Info => base.Info as WeaponInfo;

        /// <summary>
        /// Whether or not the weapon can parry other weapons and is
        /// parryable by other weapons.
        /// </summary>
        public virtual bool IsParryable { get; protected set; } = false;

        public bool IsParried { get; set; }

        public ulong ParryTimeOrigin { get; protected set; }

        public Character Character { get; set; }

        public override bool StacksWith(Item item) => false;

        public override void Equip(Character character)
        {
            GD.Print("Equipped by " + character.Name);
            Character = character;
        }

        public override void Unequip(Character character)
        {
            Character = null;
        }

        public override void Use()
        {
            RemainingUseTime = Info.UseTime;
        }

        public override void Deuse()
        {

        }

        public override void _Process(double delta)
        {
            if (RemainingUseTime > 0)
            {
                if ((RemainingUseTime -= delta) <= 0)
                {
                    Deuse();
                }
            }
        }

        public virtual void _on_hitbox_hit(BoundingBox box)
        {
            if (box is Hurtbox hurtbox)
            {
                hurtbox.InflictDamage(Info.Damage, Character, Info.Knockback);
            }
        }
    }
}
