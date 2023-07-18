using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.BoundingBoxes;

namespace SupaLidlGame.Entities;

public partial class Projectile : RigidBody2D
{
    public Vector2 Velocity => Direction * Speed;

    [Export]
    public float Speed { get; set; }

    [Export]
    public Vector2 Direction { get; set; }

    [Export]
    public Hitbox Hitbox { get; set; }

    [Export]
    public double Lifetime { get; set; } = 10;

    [Export]
    public double Delay { get; set; } = 0;

    public Character Character { get; set; }

    public override void _Ready()
    {
        Hitbox.Hit += OnHit;
    }

    public override void _Process(double delta)
    {
        if (Delay > 0)
        {
            Delay -= delta;
        }
        if ((Lifetime -= delta) <= 0)
        {
            QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Delay <= 0 ? Velocity : Vector2.Zero;
        MoveAndCollide(velocity * (float)delta);
    }

    public void OnHit(BoundingBox box)
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
