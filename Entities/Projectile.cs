using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.BoundingBoxes;

namespace SupaLidlGame.Entities
{
    public partial class Projectile : RigidBody2D
    {
        public Vector2 Velocity => Direction * Speed;

        [Export]
        public float Speed { get; set; }

        [Export]
        public Vector2 Direction { get; set; }

        [Export]
        public Hitbox Hitbox { get; set; }

        public Character Character { get; set; }

        public override void _PhysicsProcess(double delta)
        {
            Vector2 velocity = Velocity;
            MoveAndCollide(velocity * (float)delta);
        }

        public void _on_hitbox_hit(BoundingBox box)
        {
            if (box is Hurtbox hurtbox)
            {
                hurtbox.InflictDamage(
                    Hitbox.Damage,
                    Character,
                    Hitbox.Knockback,
                    knockbackVector: Direction
                );
            }
        }
    }
}
