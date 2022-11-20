using System.Collections.Generic;
using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.Utils;

namespace SupaLidlGame.BoundingBoxes
{
    public partial class Hitbox : Area2D, IFaction
    {
        private HashSet<Hurtbox> _ignoreList = new HashSet<Hurtbox>();

        [Export]
        public float Damage { get; set; }

        /// <summary>
        /// Getter/setter for the CollisionShape2D's Disabled property.
        /// </summary>
        [Export]
        public bool IsDisabled
        {
            get => _collisionShape.Disabled;
            set => _collisionShape.Disabled = value;
        }

        [Export]
        public float Knockback { get; set; }

        [Export]
        public ushort Faction { get; set; }

        public Character Inflictor { get; set; }

        private CollisionShape2D _collisionShape;

        public override void _Ready()
        {
            _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        }

        public void _on_area_entered(Area2D area)
        {
            if (area is Hurtbox hurtbox)
            {
                // we don't want to hurt teammates
                if (Faction != hurtbox.Faction)
                {
                    if (!_ignoreList.Contains(hurtbox))
                    {
                        _ignoreList.Add(hurtbox);
                        hurtbox.InflictDamage(Damage, Inflictor, Knockback);
                    }
                }
            }
        }

        public void ResetIgnoreList() => _ignoreList.Clear();
    }
}
