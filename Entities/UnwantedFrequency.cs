using Godot;

namespace SupaLidlGame.Entities;

public partial class UnwantedFrequency : Projectile, Utils.ITarget
{
    [Export]
    public Characters.Character CharacterTarget { get; set; }

    [Export]
    public float HomingVelocity { get; set; } = 1;

    public Utils.Trail Trail { get; private set; }

    public Node2D TrailRotation { get; private set; }

    public Node2D TrailPosition { get; private set; }

    public GpuParticles2D DeathParticles { get; private set; }

    public GpuParticles2D SpawnParticles { get; private set; }

    public Timer DeferDeathTimer { get; private set; }

    public SceneTreeTimer DeferNeutralTeamTimer { get; private set; }

    private double _currentLifetime = 0;

    public override void _Ready()
    {
        TrailRotation = GetNode<Node2D>("TrailRotation");
        TrailPosition = TrailRotation.GetNode<Node2D>("TrailPosition");
        Trail = TrailPosition.GetNode<Utils.Trail>("Trail");
        DeferDeathTimer = GetNode<Timer>("DeferDeath");
        DeathParticles = GetNode<GpuParticles2D>("DeathParticles");
        SpawnParticles = GetNode<GpuParticles2D>("SpawnParticles");
        SpawnParticles.Emitting = true;
        Hitbox.Hit += (BoundingBoxes.BoundingBox box) =>
        {
            if (box is BoundingBoxes.Hurtbox && box.Faction != Hitbox.Faction)
            {
                Die();
            }
        };
        base._Ready();
    }

    public override void _Process(double delta)
    {
        _currentLifetime += delta;
        float radians = (float)_currentLifetime * 24;
        TrailRotation.Rotation = Direction.Angle();
        TrailPosition.Position = new Vector2(0, 4 * Mathf.Sin(radians));

        if (CharacterTarget is not null)
        {
            var pos = CharacterTarget.GlobalPosition;
            var desired = GlobalPosition.DirectionTo(pos);
            Direction += (desired - Direction) * HomingVelocity * (float)delta;
        }

        base._Process(delta);
    }

    public override void Die()
    {
        IsDead = Trail.IsDead = true;
        Hitbox.SetDeferred("monitoring", false);
        DeferDeathTimer.Timeout += () =>
        {
            QueueFree();
        };
        DeferDeathTimer.Start();
        DeathParticles.Emitting = true;
        SpawnParticles.Emitting = false;
        GetNode<AudioStreamPlayer2D>("Sound").Stop();
        GetNode<AnimationPlayer>("AnimationPlayer").Play("death");
    }
}
