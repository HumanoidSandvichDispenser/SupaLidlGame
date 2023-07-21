using Godot;
using GodotUtilities;

namespace SupaLidlGame.Characters;

public partial class Doc : Enemy
{
    [Export]
    public State.NPC.NPCStateMachine BossStateMachine { get; set; }

    public int Intensity
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

    public override void _Ready()
    {
        base._Ready();
    }
    
    public override void _Process(double delta)
    {
        BossStateMachine.Process(delta);
        base._Process(delta);
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
