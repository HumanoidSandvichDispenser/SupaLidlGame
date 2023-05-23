using Godot;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items
{
    public abstract partial class Weapon : Item
    {
        public double RemainingUseTime { get; protected set; } = 0;

        public override bool IsUsing => RemainingUseTime > 0;

        /// <summary>
        /// How much damage in HP that this weapon deals.
        /// </summary>
        [Export]
        public float Damage { get; set; } = 0;

        /// <summary>
        /// The time in seconds it takes for this weapon to become available
        /// again after using.
        /// </summary>
        [Export]
        public double UseTime { get; set; } = 0;

        /// <summary>
        /// The magnitude of the knockback force of the weapon.
        /// </summary>
        [Export]
        public float Knockback { get; set; } = 0;

        /// <summary>
        /// The initial velocity of any projectile the weapon may spawn.
        /// </summary>
        [Export]
        public float InitialVelocity { get; set; } = 0;

        public virtual bool IsParryable { get; protected set; } = false;

        public bool IsParried { get; set; }

        public Character Character { get; set; }

        [Export]
        public float MinDistanceHint { get; set; }

        [Export]
        public float MaxDistanceHint { get; set; }

        [Export]
        public Node2D Anchor { get; set; }

        public override bool StacksWith(Item item) => false;

        public override void Equip(Character character)
        {
            Character = character;
        }

        public override void Unequip(Character character)
        {
            Character = null;
        }

        public override void Use()
        {
            RemainingUseTime = UseTime;
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
                hurtbox.InflictDamage(Damage, Character, Knockback);
            }
        }
    }
}
