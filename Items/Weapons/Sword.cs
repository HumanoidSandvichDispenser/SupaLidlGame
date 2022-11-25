using Godot;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Characters;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Items.Weapons
{
    public partial class Sword : Weapon
    {
        public bool IsAttacking { get; protected set; }

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

        [Export]
        public CPUParticles2D ParryParticles { get; set; }

        public override bool IsParryable { get; protected set; }

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

            IsParried = false;

            AnimationPlayer.Stop();
            IsParryable = true;

            if (GetNode<Node2D>("Anchor").Rotation < Mathf.DegToRad(50))
            {
                AnimationPlayer.Play("use");
            }
            else
            {
                AnimationPlayer.Play("use2");
            }

            base.Use();
        }

        public override void Deuse()
        {
            //AnimationPlayer.Stop();
            Deattack();
            base.Deuse();
        }

        public void Attack()
        {
            ParryTimeOrigin = Time.GetTicksMsec();
            //RemainingAttackTime = AttackTime;
            IsAttacking = true;
            Hitbox.IsDisabled = false;
        }

        public void Deattack()
        {
            IsAttacking = false;
            IsParryable = false;
            Hitbox.IsDisabled = true;
            ProcessHits();
            Hitbox.ResetIgnoreList();
            AnimationPlayer.PlaybackSpeed = 1;
        }

        public override void _Ready()
        {
            Hitbox.Damage = Damage;
        }

        public override void _Process(double delta)
        {
            /*
            if (RemainingAttackTime > 0)
            {
                if ((RemainingAttackTime -= delta) <= 0)
                {
                    Deattack();
                }
            }
            */
            base._Process(delta);
        }

        public void ProcessHits()
        {
            if (IsParried)
            {
                return;
            }

            foreach (BoundingBox box in Hitbox.Hits)
            {
                GD.Print("processing hit");
                if (box is Hurtbox hurtbox)
                {
                    hurtbox.InflictDamage(Damage, Character, Knockback);
                }
            }
        }

        public void AttemptParry(Weapon otherWeapon)
        {
            if (IsParryable && otherWeapon.IsParryable)
            {
                ParryParticles.Emitting = true;
                if (ParryTimeOrigin < otherWeapon.ParryTimeOrigin)
                {
                    // our character was parried
                    IsParried = true;
                    AnimationPlayer.PlaybackSpeed = 0.25f;
                    Character.Stun(1.5f);
                }
            }
            //this.GetAncestor<TileMap>().AddChild(instance);
        }

        public override void _on_hitbox_hit(BoundingBox box)
        {
            if (IsParried)
            {
                return;
            }

            if (box is Hitbox hb)
            {
                Weapon w = hb.GetAncestor<Weapon>();
                if (w is not null)
                {
                    //Vector2 a = new Vector2(2, 2) * new Vector2(5, 2);
                    AttemptParry(w);
                }
            }

            if (box is Hurtbox hurt)
            {
                if (hurt.GetParent() is Character c)
                {
                    var item = c.Inventory.SelectedItem;
                    if (item is Weapon w)
                    {
                        AttemptParry(w);
                    }
                }
            }

            //base._on_hitbox_hit(box);
        }
    }
}
