using Godot;

namespace SupaLidlGame.Items.Weapons
{
    public partial class MeleeInfo : WeaponInfo
    {
        /// <summary>
        /// The time frame in seconds for which the weapon will deal damage.
        /// </summary>
        /// <remarks>
        /// The value of <c>AttackTime</c> should be less than the
        /// value of <c>UseTime</c>
        /// </remarks>
        [Export]
        public double AttackTime { get; set; } = 0;
    }
}
