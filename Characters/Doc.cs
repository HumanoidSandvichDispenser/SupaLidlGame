using Godot;
using GodotUtilities;

namespace SupaLidlGame.Characters;

public partial class Doc : Boss
{
    public AnimationPlayer TelegraphAnimation { get; set; }

    public override float Health
    {
        get => base.Health;
        set
        {
            if (IsActive)
            {
                base.Health = value;
            }
            else
            {
                // play opening animation
                // then become active when it finishes
                base.Health = value;
            }
        }
    }

    public override int Intensity
    {
        get
        {
            switch (Health)
            {
                case < 250:
                    return 3;
                case < 500:
                    return 2;
                default:
                    return 1;
            }
        }
    }

    public Doc()
    {
        ShouldMove = false;
    }

    public override void _Ready()
    {
        TelegraphAnimation = GetNode<AnimationPlayer>("Animations/Telegraph");
        base._Ready();
    }
    
    public override void _Process(double delta)
    {
        if (IsActive)
        {
            BossStateMachine.Process(delta);
        }
        base._Process(delta);
    }

    protected override float ReceiveDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackDir = default)
    {
        if (IsActive)
        {
            return base.ReceiveDamage(
                damage, inflictor, knockback, knockbackDir);
        }

        return 1;
    }

    public override void OnReceivedDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackDir = default)
    {
        GetNode<GpuParticles2D>("Effects/HurtParticles")
            .SetDirection(knockbackDir);

        base.OnReceivedDamage(damage, inflictor, knockback, knockbackDir);
    }
}
