using Godot;
using GodotUtilities;
using SupaLidlGame.Utils;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Characters;

[Scene]
public sealed partial class Player : Character
{
    private string _spriteAnim;

    [Export]
    public PlayerCamera Camera { get; set; }

    [Export]
    public Marker2D DirectionMarker { get; private set; }

    [Export]
    public AnimationTree AnimationTree { get; private set; }

    public InteractionRay InteractionRay { get; private set; }

    public override void _Ready()
    {
        InteractionRay = GetNode<InteractionRay>("Direction2D/InteractionRay");

        base._Ready();

        Inventory.UsedItem += (Items.Item item) =>
        {
            if (item is Items.Weapons.Sword)
            {
                AttackAnimation.Play("sword");
            }
        };

        HealthChanged += (args) =>
        {
            var signal = Events.EventBus.SignalName.PlayerHealthChanged;
            this.GetEventBus().EmitSignal(signal, args);
        };
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (StateMachine != null)
        {
            StateMachine.Input(@event);
        }
    }

    /// <summary>
    /// Respawns the player with full health and plays spawn animation
    /// </summary>
    public void Spawn()
    {
        Health = 100;
        HurtAnimation.Play("spawn");
    }

    public override void ModifyVelocity()
    {
        base.ModifyVelocity();
    }

    public override void Stun(float time)
    {
        base.Stun(time);
        Camera.Shake(2, 0.8f);
        // TODO: implement visual effects for stun
    }

    protected override void OnReceivedDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackDir = default)
    {
        if (damage >= 10 && IsAlive)
        {
            Camera.Shake(2, 0.5f);
        }

        GetNode<GpuParticles2D>("Effects/HurtParticles")
            .SetDirection(knockbackDir);

        base.OnReceivedDamage(damage, inflictor, knockback, knockbackDir);
    }

    public override void Die()
    {
        HurtAnimation.Play("death");
    }

    protected override void DrawTarget()
    {
        base.DrawTarget();
        DirectionMarker.GlobalRotation = DirectionMarker.GlobalPosition
            .DirectionTo(GetGlobalMousePosition())
            .Angle();
    }

    public override void Footstep()
    {
        GetNode<AudioStreamPlayer2D>("Effects/Footstep")
            .OnWorld()
            .WithPitchDeviation(0.125f)
            .Play();
    }
}
