using Godot;
using GodotUtilities;
using SupaLidlGame.Extensions;
using SupaLidlGame.Items;
using SupaLidlGame.Utils;
using SupaLidlGame.State.Character;

namespace SupaLidlGame.Characters;

public partial class Character : CharacterBody2D, IFaction
{
    [Export]
    public float Speed { get; protected set; } = 32.0f;

    [Export]
    public float Friction { get; protected set; } = 4.0f;

    [Export]
    public float Mass
    {
        get => _mass;
        set
        {
            if (value > 0)
                _mass = value;
        }
    }

    [Signal]
    public delegate void HurtEventHandler(Events.HealthChangedArgs args);

    [Signal]
    public delegate void DeathEventHandler(Events.HealthChangedArgs args);

    protected float _mass = 1.0f;

    public Vector2 NetImpulse { get; set; } = Vector2.Zero;

    public Vector2 Direction { get; set; } = Vector2.Zero;

    public Vector2 Target { get; set; } = Vector2.Zero;

    [Export]
    public float Health
    {
        get => _health;
        set
        {
            if (!IsAlive && value < 0)
            {
                return;
            }

            _health = value;
            if (_health <= 0)
            {
                Die();
            }
        }
    }

    public bool IsAlive => Health > 0;

    protected float _health = 100f;

    public double StunTime { get; set; }

    [Export]
    public Sprite2D Sprite { get; set; }

    [Export]
    public Inventory Inventory { get; set; }

    [Export]
    public CharacterStateMachine StateMachine { get; set; }

    [Export]
    public BoundingBoxes.Hurtbox Hurtbox { get; set; }

    [Export]
    public ushort Faction { get; set; }

    public AnimationPlayer MovementAnimation { get; set; }

    public AnimationPlayer HurtAnimation { get; set; }

    public override void _Ready()
    {
        // TODO: 80+ char line
        MovementAnimation = GetNode<AnimationPlayer>("Animations/Movement");
        HurtAnimation = GetNode<AnimationPlayer>("Animations/Hurt");
        GD.Print(Name + " " + MovementAnimation.CurrentAnimation);
        Hurtbox.ReceivedDamage += OnReceivedDamage;
    }

    public override void _Process(double delta)
    {
        if (StateMachine != null)
        {
            StateMachine.Process(delta);
        }

        Sprite.FlipH = Target.X < 0;
        DrawTarget();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (StateMachine != null)
        {
            StateMachine.PhysicsProcess(delta);
        }
    }

    /// <summary>
    /// Modify the <c>Character</c>'s velocity
    /// </summary>
    public virtual void ModifyVelocity()
    {
        if (StunTime > 0)
        {
            Velocity *= 0.25f;
        }
    }

    public virtual void Die()
    {
        GD.Print("lol died");
        QueueFree();
    }

    public void ApplyImpulse(Vector2 impulse, bool resetVelocity = false)
    {
        // delta p = F delta t
        if (resetVelocity)
            Velocity = Vector2.Zero;
        NetImpulse += impulse / Mass;
    }

    public virtual void Stun(float time)
    {
        StunTime += time;
    }

    protected virtual void DrawTarget()
    {
        Vector2 target = Target;
        float angle = Mathf.Atan2(target.Y, Mathf.Abs(target.X));
        Vector2 scale = Inventory.Scale;
        if (target.X < 0)
        {
            scale.Y = -1;
            angle = Mathf.Pi - angle;
        }
        else
        {
            scale.Y = 1;
        }
        Inventory.Scale = scale;
        Inventory.Rotation = angle;
    }

    public void UseCurrentItem()
    {
        if (StunTime > 0)
        {
            GD.Print("tried to use weapon but stunned");
            return;
        }

        if (Inventory.SelectedItem is Weapon weapon)
        {
            weapon.Use();
            if (weapon.IsUsing)
            {
                Inventory.EmitSignal(Inventory.SignalName.UsedItem, weapon);
            }
        }
    }

    public void LookTowardsDirection()
    {
        if (!Direction.IsZeroApprox())
        {
            Target = Direction;
        }
    }

    public virtual void OnReceivedDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackDir = default)
    {
        if (Health <= 0)
        {
            return;
        }

        float oldHealth = Health;
        Health -= damage;

        // create damage text
        var textScene = GD.Load<PackedScene>("res://UI/FloatingText.tscn");
        var instance = textScene.Instantiate<UI.FloatingText>();
        instance.Text = Mathf.Round(damage).ToString();
        instance.GlobalPosition = GlobalPosition;
        this.GetAncestor<TileMap>().AddChild(instance);

        // apply knockback

        ApplyImpulse(knockbackDir.Normalized() * knockback);

        GD.Print("lol");

        // play damage animation
        var anim = GetNode<AnimationPlayer>("Animations/Hurt");
        if (anim != null)
        {
            anim.Stop();
            anim.Play("hurt");
            anim.Queue("hurt_flash");
        }

        // if anyone involved is a player, shake their screen
        Player plr = inflictor as Player ?? this as Player;
        if (plr is not null)
        {
            plr.Camera.Shake(1, 0.4f);
        }

        if (this.GetNode("HurtSound") is AudioStreamPlayer2D sound)
        {
            // very small pitch deviation
            sound.At(GlobalPosition).WithPitchDeviation(0.125f).PlayOneShot();
        }

        Events.HealthChangedArgs args = new Events.HealthChangedArgs
        {
            Attacker = inflictor,
            OldHealth = oldHealth,
            NewHealth = Health,
            Damage = damage,
        };

        EmitSignal(SignalName.Hurt, args);

        if (Health <= 0)
        {
            EmitSignal(SignalName.Death, args);
            GetNode<GpuParticles2D>("DeathParticles")
                .CloneOnWorld<GpuParticles2D>()
                .EmitOneShot();
        }
    }
}
