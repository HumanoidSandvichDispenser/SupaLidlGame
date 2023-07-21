using Godot;
using GodotUtilities;
using SupaLidlGame.Utils;
using SupaLidlGame.BoundingBoxes;

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

    public AnimationPlayer AttackAnimation { get; set; }

    public override void _Ready()
    {
        InteractionRay = GetNode<InteractionRay>("Direction2D/InteractionRay");
        Death += (Events.HealthChangedArgs args) =>
        {
            Visible = false;
        };

        AttackAnimation = GetNode<AnimationPlayer>("Animations/Attack");

        base._Ready();

        Inventory.UsedItem += (Items.Item item) =>
        {
            if (item is Items.Weapons.Sword)
            {
                AttackAnimation.Play("sword");
            }
        };
    }

    public override void _Input(InputEvent @event)
    {
        if (StateMachine != null)
        {
            StateMachine.Input(@event);
        }
    }

    public void Spawn()
    {
        Health = 100;
        Visible = true;
    }

    public override void ModifyVelocity()
    {
        if (StateMachine.CurrentState is SupaLidlGame.State.Character.PlayerRollState)
        {
            Velocity *= 2;
        }

        base.ModifyVelocity();
    }

    public override void Stun(float time)
    {
        base.Stun(time);
        Camera.Shake(2, 0.8f);
        // TODO: implement visual effects for stun
    }

    public override void OnReceivedDamage(
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
        GD.Print("died");
        //base.Die();
    }

    protected override void DrawTarget()
    {
        base.DrawTarget();
        DirectionMarker.GlobalRotation = DirectionMarker.GlobalPosition
            .DirectionTo(GetGlobalMousePosition())
            .Angle();
    }
}
