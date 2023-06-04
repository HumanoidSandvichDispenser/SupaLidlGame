using Godot;
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
    public AnimatedSprite2D Sprite { get; set; }

    [Export]
    public Inventory Inventory { get; set; }

    [Export]
    public CharacterStateMachine StateMachine { get; set; }

    [Export]
    public ushort Faction { get; set; }

    public override void _Process(double delta)
    {
        if (StateMachine != null)
        {
            StateMachine.Process(delta);
        }

        Sprite.FlipH = Target.X < 0;
        DrawTarget();
    }

    public override void _Input(InputEvent @event)
    {
        if (StateMachine != null)
        {
            StateMachine.Input(@event);
        }
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

    protected void DrawTarget()
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
        }
    }

    public void LookTowardsDirection()
    {
        if (!Direction.IsZeroApprox())
        {
            Target = Direction;
        }
    }

    public virtual void _on_hurtbox_received_damage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackOrigin = default,
        Vector2 knockbackVector = default)
    {
        Health -= damage;

        // create damage text
        var textScene = GD.Load<PackedScene>("res://UI/FloatingText.tscn");
        var instance = textScene.Instantiate<UI.FloatingText>();
        instance.Text = Mathf.Round(damage).ToString();
        instance.GlobalPosition = GlobalPosition;
        this.GetAncestor<TileMap>().AddChild(instance);

        // apply knockback
        Vector2 knockbackDir = knockbackVector;
        if (knockbackDir == default)
        {
            if (knockbackOrigin == default)
            {
                knockbackOrigin = inflictor.GlobalPosition;
            }

            knockbackDir = knockbackOrigin.DirectionTo(GlobalPosition);
        }

        ApplyImpulse(knockbackDir.Normalized() * knockback);

        GD.Print("lol");

        // play damage animation
        var anim = GetNode<AnimationPlayer>("FlashAnimation");
        if (anim != null)
        {
            anim.Stop();
            anim.Play("Hurt");
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
            sound.At(GlobalPosition).WithPitchDeviation(0.125f).Play();
        }
    }
}
