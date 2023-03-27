using Godot;

namespace SupaLidlGame.Items
{
    public partial class WeaponInfo : ItemInfo
    {
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

    }
}
