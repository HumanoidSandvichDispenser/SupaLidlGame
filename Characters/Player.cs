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

    private Node2D _effects;

    private Node2D _characterEffects;

    [Node]
    private TargetTracer _targetTracer;

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

    [Export]
    public new PlayerStats Stats { get; private set; }

    public InteractionRay InteractionRay { get; private set; }

    public override void _Ready()
    {
        InteractionRay = GetNode<InteractionRay>("Direction2D/InteractionRay");

        _effects = GetNode<Node2D>("%Effects");

        _characterEffects = GetNode<Node2D>("%CharacterEffects");

        _targetTracer = GetNode<TargetTracer>("%TargetTracer");

        base._Ready();

        Stats = base.Stats as PlayerStats;

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

        //GD.Print("Inventory: " + Inventory.Items);
        //Inventory.AddItemToHotbar(Inventory.Items[0]);
        Inventory.SelectedIndex = 0;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        float angle = DesiredTarget.Angle();

        if (Inventory.IsUsingItem)
        {
            _targetTracer.Intensity = 1;
        }
        else
        {
            // must turn > pi / 4 radians per second to increase intensity
            float deltaTheta = Mathf.Abs(_targetTracer.Rotation - angle);
            _targetTracer.Intensity = Mathf.Min(_targetTracer.Intensity +
                deltaTheta, 1);
            _targetTracer.Intensity = Mathf.Max(_targetTracer.Intensity -
                Mathf.Pi / 4 * (float)delta, 0);
        }

        _targetTracer.Rotation = angle;
    }

    public override void _Input(InputEvent @event)
    {
        if (StateMachine != null)
        {
            StateMachine.Input(@event);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (StateMachine != null)
        {
            StateMachine.UnhandledInput(@event);
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

    public override void Stun(double time)
    {
        base.Stun(time);
        Camera.Shake(2, 0.8f);
        Events.EventBus.Instance.EmitSignal(
            Events.EventBus.SignalName.PlayerStun);
    }

    protected override void OnReceivedDamage(
        float damage,
        Character inflictor,
        float knockback,
        Items.Weapon weapon = null,
        Vector2 knockbackDir = default)
    {
        if (StateMachine.CurrentState is State.Character.PlayerRollState)
        {
            // dodge dots:
            // melee: 
            float dot = Direction.Dot(knockbackDir);
            GD.Print(dot);
            if (weapon is Items.Weapons.Sword)
            {
                // if melee weapon then check if dot is away
                GD.Print("sword");
                if (dot > Utils.Physics.COS_30_DEG)
                {
                    // ignore hit
                    GD.Print("ignore hit");
                    return;
                }
            }
        }

        if (damage >= 10 && IsAlive)
        {
            Camera.Shake(2, 0.5f);
        }

        GetNode<GpuParticles2D>("Effects/HurtParticles")
            .SetDirection(knockbackDir);

        Events.EventBus.Instance.EmitSignal(
            Events.EventBus.SignalName.PlayerHurt,
            new Events.HurtArgs { Attacker = inflictor, Weapon = weapon }
        );

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
        if (Target.X < 0)
        {
            _characterEffects.Scale = new Vector2(-1, 1);
        }
        else if (Target.X > 0)
        {
            _characterEffects.Scale = new Vector2(1, 1);
        }
    }

    public override void Footstep()
    {
        GetNode<AudioStreamPlayer2D>("Effects/Footstep")
            .OnWorld()
            .WithPitchDeviation(0.125f)
            .Play();
    }

    public override void UseCurrentItemAlt()
    {
        // must have at least 1 level to use
        if (Stats.Level.Value >= 1)
        {
            if (Inventory.SelectedItem is Items.Weapon weapon)
            {
                if (!weapon.IsUsingAlt)
                {
                    Stats.Level.Value--;
                }
            }
            base.UseCurrentItemAlt();
        }
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
                GD.Print(joystick);
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
