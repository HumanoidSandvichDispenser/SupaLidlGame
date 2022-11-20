using Godot;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items.Weapons
{
    public partial class Sword : Weapon
    {
        public double RemainingAttackTime { get; protected set; } = 0;

        public bool IsAttacking => RemainingAttackTime > 0;

        [Export]
        public Hitbox Hitbox { get; set; }

        [Export]
        public AnimationPlayer AnimationPlayer { get; set; }

        /// <summary>
        /// The time frame in seconds for which the weapon will deal damage.
        /// </summary>
        /// <remarks>
        /// The value of <c>AttackTime</c> should be less than the
        /// value of <c>UseTime</c>
        /// </remarks>
        [Export]
        public double AttackTime { get; set; } = 0;

        public override void Equip(Character character)
        {
            Visible = true;
            base.Equip(character);
            Hitbox.Faction = character.Faction; // character is null before base
        }

        public override void Unequip(Character character)
        {
            Visible = false;
            base.Unequip(character);
        }

        public override void Use()
        {
            if (RemainingUseTime > 0)
            {
                return;
            }

            RemainingAttackTime = AttackTime;

            AnimationPlayer.Play("use");
            Hitbox.IsDisabled = false;
            base.Use();
        }

        public override void Deuse()
        {
            AnimationPlayer.Stop();
            Deattack();
            base.Deuse();
        }

        protected void Deattack()
        {
            Hitbox.IsDisabled = true;
            Hitbox.ResetIgnoreList();
        }

        public override void _Ready()
        {
            Hitbox.Damage = Damage;
        }

        public override void _Process(double delta)
        {
            if (RemainingAttackTime > 0)
            {
                if ((RemainingAttackTime -= delta) <= 0)
                {
                    Deattack();
                }
            }
            base._Process(delta);
        }
    }
}
