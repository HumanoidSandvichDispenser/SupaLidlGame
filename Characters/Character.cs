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
    public delegate void HealthChangedEventHandler(Events.HealthChangedArgs args);

    [Signal]
    public delegate void HurtEventHandler(Events.HurtArgs args);

    [Signal]
    public delegate void DeathEventHandler(Events.HurtArgs args);

    protected float _mass = 1.0f;

    public Vector2 NetImpulse { get; set; } = Vector2.Zero;

    public Vector2 Direction { get; set; } = Vector2.Zero;

    public Vector2 Target { get; set; } = Vector2.Zero;
    
    [Export]
    public Texture2D HandTexture { get; set; }

    [Export]
    public virtual float Health
    {
        get => _health;
        set
        {
            if (!IsAlive && value < 0)
            {
                return;
            }

            var args = new Events.HealthChangedArgs
            {
                OldHealth = _health,
                NewHealth = value,
            };
            EmitSignal(SignalName.HealthChanged, args);

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

    public AnimationPlayer StunAnimation { get; set; }

    public AnimationPlayer AttackAnimation { get; set; }

    private UI.DamageText _curDamageText;

    public override void _Ready()
    {
        // TODO: 80+ char line
        MovementAnimation = GetNode<AnimationPlayer>("Animations/Movement");
        HurtAnimation = GetNode<AnimationPlayer>("Animations/Hurt");
        StunAnimation = GetNode<AnimationPlayer>("Animations/Stun");
        AttackAnimation = GetNode<AnimationPlayer>("Animations/Attack");

        Hurtbox.ReceivedDamage += OnReceivedDamage;
    }

    public override void _Process(double delta)
    {
        if (StateMachine != null)
        {
            StateMachine.Process(delta);
        }

        if (StunTime > 0 && !StunAnimation.IsPlaying())
        {
            StunAnimation.TryPlayAny("stun", "npc_stun/stun");
        }
        else if (StunTime < 0 && StunAnimation.IsPlaying())
        {
            StunAnimation.Stop();
        }

        if (Target.X < 0)
        {
            Sprite.FlipH = true;
        }
        else if (Target.X > 0)
        {
            Sprite.FlipH = false;
        }
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

        var state = StateMachine.CurrentState;
        if (state is State.Character.CharacterDashState dashState)
        {
            Velocity *= dashState.VelocityModifier;
        }
        // TODO: make PlayerRollState a CharacterRollState instead
        else if (state is State.Character.PlayerRollState rollState)
        {
            Velocity *= 2;
            //Velocity *= rollState.VelocityModifier;
        }
    }

    /// <summary>
    /// Handles the <c>Character</c>'s death.
    /// </summary>
    public virtual void Die()
    {
        if (HurtAnimation.TryPlayAny(out var name, "death", "npc_hurt/death"))
        {
            HurtAnimation.Play(name);
            HurtAnimation.AnimationFinished += (StringName name) =>
                QueueFree();
        }
        else
        {
            QueueFree();
        }
    }

    public void ApplyImpulse(Vector2 impulse, bool resetVelocity = false)
    {
        // delta p = F delta t
        if (resetVelocity)
            Velocity = Vector2.Zero;
        NetImpulse += impulse / Mass;
    }

    /// <summary>
    /// Stuns the <c>Chararacter</c> for an amount of time. If
    /// <paramref name="time"/> is less than the <c>Character</c>'s current
    /// stun time left, it will have no effect.
    /// </summary>
    public virtual void Stun(float time)
    {
        StunTime = Mathf.Max(time, StunTime);
    }

    /// <summary>
    /// Draws the character so that its sprite and inventory items face the
    /// character's direction.
    /// </summary>
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
        else if (target.X > 0)
        {
            scale.Y = 1;
        }
        Inventory.Scale = scale;
        Inventory.Rotation = angle;
    }

    /// <summary>
    /// Use the current item the character is using. Prefer to call this over
    /// <c>Item.Use</c> as it will check if the character is stunned or alive.
    /// </summary>
    public void UseCurrentItem()
    {
        if (StunTime > 0 || !IsAlive)
        {
            return;
        }

        if (Inventory.SelectedItem is Weapon weapon)
        {
            weapon.Use();
        }
    }

    public void DeuseCurrentItem()
    {
        if (Inventory.SelectedItem is Weapon weapon)
        {
            weapon.Deuse();
            // TODO: DeusedItem signal, implement when needed
        }
    }

    public virtual void UseCurrentItemAlt()
    {
        if (StunTime > 0 || !IsAlive)
        {
            return;
        }

        if (Inventory.SelectedItem is Weapon weapon)
        {
            weapon.UseAlt();
        }
    }

    public void DeuseCurrentItemAlt()
    {
        if (Inventory.SelectedItem is Weapon weapon)
        {
            weapon.DeuseAlt();
        }
    }

    public void LookTowardsDirection()
    {
        if (!Direction.IsZeroApprox())
        {
            Target = Direction;
        }
    }

    /// <summary>
    /// Override this method to modify the damage the character takes.
    /// </summary>
    protected virtual float ReceiveDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackDir = default) => damage;

    protected void CreateDamageText(float damage)
    {
        // create damage text
        var textScene = GD.Load<PackedScene>("res://UI/DamageText.tscn");
        if (_curDamageText is null || !IsInstanceValid(_curDamageText))
        {
            _curDamageText = textScene.Instantiate<UI.DamageText>();
            this.GetWorld().CurrentMap.AddChild(_curDamageText);
        }
        _curDamageText.Damage += damage;
        _curDamageText.Timer.Start();
        _curDamageText.GlobalPosition = GlobalPosition;
        _curDamageText.ShowText();
    }

    /// <summary>
    /// Handles the character taking damage.
    /// </summary>
    protected virtual void OnReceivedDamage(
        float damage,
        Character inflictor,
        float knockback,
        Weapon weapon = null,
        Vector2 knockbackDir = default)
    {
        if (Health <= 0)
        {
            return;
        }

        float oldHealth = Health;
        damage = ReceiveDamage(damage, inflictor, knockback, knockbackDir);
        Health -= damage;

        var hurtParticles = GetNode<GpuParticles2D>("Effects/HurtParticles");
        if (hurtParticles is not null)
        {
            hurtParticles.SetDirection(knockbackDir);
        }

        CreateDamageText(damage);

        // apply knockback
        ApplyImpulse(knockbackDir.Normalized() * knockback);

        // play damage animation
        if (HurtAnimation is not null && Health > 0)
        {
            HurtAnimation.Stop();
            HurtAnimation.TryPlayAny("hurt", "npc_hurt/hurt");
            HurtAnimation.TryQueue("hurt_flash");
        }

        if (this.GetNode("Effects/HurtSound") is AudioStreamPlayer2D sound)
        {
            // very small pitch deviation
            sound.At(GlobalPosition).WithPitchDeviation(0.125f).PlayOneShot();
        }

        Events.HurtArgs args = new Events.HurtArgs
        {
            Attacker = inflictor,
            OldHealth = oldHealth,
            NewHealth = Health,
            Weapon = weapon,
            Damage = damage,
        };

        EmitSignal(SignalName.Hurt, args);

        if (inflictor is Player)
        {
            EmitPlayerHitSignal(args);
        }

        if (Health <= 0)
        {
            EmitSignal(SignalName.Death, args);
            GetNode<GpuParticles2D>("DeathParticles")?
                .CloneOnWorld<GpuParticles2D>()
                .EmitOneShot();
        }
    }

    /// <summary>
    /// Converts a HurtArgs to HitArgs if the attacker was a player and emits
    /// the <c>EventBus.PlayerHit</c> signal.
    /// </summary>
    private void EmitPlayerHitSignal(Events.HurtArgs args)
    {
        var newArgs = new Events.HitArgs
        {
            OldHealth = args.OldHealth,
            NewHealth = args.NewHealth,
            Damage = args.Damage,
            Weapon = args.Weapon,
            Victim = this,
        };

        var bus = Events.EventBus.Instance;
        bus.EmitSignal(Events.EventBus.SignalName.PlayerHit, newArgs);
    }

    /// <summary>
    /// Plays a footstep sound. This should be called through an
    /// <c>AnimationPlayer</c> to sync sounds with animations.
    /// </summary>
    public virtual void Footstep()
    {
        if (GetNode("Effects/Footstep") is AudioStreamPlayer2D player)
        {
            player.Play();
        }
    }

    /// <summary>
    /// Returns whether the <c>Character</c> has line of sight with
    /// <paramref name="character"/>.
    /// </summary>
    /// <param name="character">The character to check for LOS</param>
    /// <param name="excludeClip">
    /// Determines whether the raycast should pass through world clips (physics
    /// layer 5)
    /// </param>
    public bool HasLineOfSight(Character character, bool excludeClip = false)
    {
        var exclude = new Godot.Collections.Array<Godot.Rid>();
        exclude.Add(GetRid());
        var rayParams = new PhysicsRayQueryParameters2D
        {
            Exclude = exclude,
            From = GlobalPosition,
            To = character.GlobalPosition,
            //CollisionMask = 1 + (uint)(excludeClip ? 0 : 16),
            CollisionMask = 1,
        };
        var spaceState = GetWorld2D().DirectSpaceState;
        var result = spaceState.IntersectRay(rayParams);
        return result.Count == 0;
    }
}
