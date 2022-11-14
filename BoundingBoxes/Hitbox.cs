using System.Collections.Generic;
using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.BoundingBoxes
{
    public partial class Hitbox : Area2D
    {
        private HashSet<Hurtbox> _ignoreList = new HashSet<Hurtbox>();

        [Export]
        public float Damage { get; set; } = 0;

        [Export]
        public bool IsEnabled { get; set; }

        [Export]
        public float Knockback { get; set; }

        public Character Inflictor { get; set; }

        public void _on_area_entered(Area2D area)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (area is Hurtbox hurtbox)
            {
                if (!_ignoreList.Contains(hurtbox))
                {
                    _ignoreList.Add(hurtbox);
                    hurtbox.InflictDamage(Damage, Inflictor, Knockback);
                }
            }
        }

        public void ResetIgnoreList() => _ignoreList.Clear();
    }
}
