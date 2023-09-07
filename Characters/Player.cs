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

    private Vector2 _desiredTarget;

    public Vector2 DesiredTarget
    {
        get => _desiredTarget;
        set
        {
            if (value.IsZeroApprox())
            {
                return;
            }
            _desiredTarget = value;
        }
    }

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
        Items.Weapon weapon = null,
        Vector2 knockbackDir = default)
    {
        if (damage >= 10 && IsAlive)
        {
            Camera.Shake(2, 0.5f);
        }

        GetNode<GpuParticles2D>("Effects/HurtParticles")
            .SetDirection(knockbackDir);

        base.OnReceivedDamage(damage,
            inflictor,
            knockback,
            weapon,
            knockbackDir);
    }

    public override void Die()
    {
        HurtAnimation.Play("death");
    }

    protected override void DrawTarget()
    {
        base.DrawTarget();
        DirectionMarker.GlobalRotation = DesiredTarget.Angle();
    }

    public override void Footstep()
    {
        GetNode<AudioStreamPlayer2D>("Effects/Footstep")
            .OnWorld()
            .WithPitchDeviation(0.125f)
            .Play();
    }

    public Vector2 GetDesiredInputFromInput()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2 dirToMouse = GlobalPosition.DirectionTo(mousePos);
        Vector2 joystick = Godot.Input.GetVector("look_left", "look_right",
                                                 "look_up", "look_down");

        var inputMethod = Utils.World.Instance.GlobalState
            .Settings.InputMethod;
        switch (inputMethod)
        {
            case State.Global.InputMethod.Joystick:
                if (joystick.IsZeroApprox())
                {
                    return Direction;
                }
                return joystick;
            default:
                return dirToMouse;
        }
    }
}
