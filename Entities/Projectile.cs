using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.BoundingBoxes;

namespace SupaLidlGame.Entities;

public partial class Projectile : RigidBody2D
{
    [Signal]
    public delegate void HitEventHandler(BoundingBox box);

    //public virtual Vector2 Velocity => Direction * Speed;
    public virtual Vector2 Velocity
    {
        get => Direction * Speed;
        set
        {
            throw new System.NotImplementedException();
        }
    }

    [Export]
    public string ProjectileName { get; set; }

    [Export]
    public float Speed { get; set; }

    [Export]
    public Vector2 AccelerationDirection { get; set; }

    [Export]
    public float AccelerationMagnitude { get; set; }

    [Export]
    public Vector2 Direction { get; set; }

    [Export]
    public Hitbox Hitbox { get; set; }

    [Export]
    public double Lifetime { get; set; } = 10;

    [Export]
    public double Delay { get; set; } = 0;

    [System.Obsolete]
    public Character Character { get; set; }

    public Items.Weapon Weapon { get; set; }

    public bool IsDead { get; set; }

    public override void _Ready()
    {
        Hitbox.Hit += OnHit;
    }

    public override void _Process(double delta)
    {
        if (Delay > 0)
        {
            Delay -= delta;
            if (Delay <= 0)
            {
                OnDelayEnd();
            }
        }
        if ((Lifetime -= delta) <= 0)
        {
            if (!IsDead)
            {
                Die();
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsDead)
        {
            return;
        }
        Vector2 velocity = Delay <= 0 ? Velocity : Vector2.Zero;
        MoveAndCollide(velocity * (float)delta);
    }

    public void OnHit(BoundingBox box)
    {
        if (box is Hurtbox hurtbox)
        {
            hurtbox.InflictDamage(
                Hitbox.Damage,
                Hitbox.Inflictor,
                Hitbox.Knockback,
                weapon: Weapon,
                knockbackVector: Direction
            );
            EmitSignal(SignalName.Hit, box);
        }
    }

    public virtual void Die()
    {
        QueueFree();
    }

    public virtual void OnDelayEnd()
    {

    }
}
