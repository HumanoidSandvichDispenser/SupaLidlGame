using Godot;
using GodotUtilities;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Characters;
using SupaLidlGame.Extensions;
using SupaLidlGame.State.Weapon;

namespace SupaLidlGame.Items.Weapons;

/// <summary>
/// A basic melee weapon.
/// </summary>
public partial class Sword : Weapon, IParryable
{
    public bool IsAttacking { get; protected set; }

    public override bool IsUsing => IsUsingPrimary || IsUsingAlt;

    public override bool IsUsingPrimary => StateMachine.CurrentState is
        SwordAttackState;

    public override bool IsUsingAlt => StateMachine.CurrentState is
        SwordBlockState;

    [Export]
    public Hitbox Hitbox { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayer { get; set; }

    /// <summary>
    /// The time frame in seconds for which the weapon will deal damage.
    /// </summary>
    /// <remarks>
    /// The value of <c>AttackTime</c> should be less than the
    /// value of <c>UseTime</c>
    /// </remarks>
    [Export]
    public double AttackTime { get; set; } = 0;

    [Export]
    public double AttackAltTime { get; set; } = 0;

    [Export]
    public double AttackAnimationDuration { get; set; }

    [Export]
    public GpuParticles2D ParryParticles { get; set; }

    [Export]
    public double NPCAnticipateTime { get; set; }

    [Export]
    public WeaponStateMachine StateMachine { get; set; }

    [Export]
    public Node2D Anchor { get; set; }

    public bool HasParried { get; protected set; }

    public override bool IsParryable { get; protected set; }

    public ulong ParryTimeOrigin { get; protected set; }

    [Export]
    public float BlockForce { get; set; } = 1;

    private Tween _currentTween;

    private AnimationNodeStateMachinePlayback _playback;

    public override void Equip(Character character)
    {
        base.Equip(character);
        Hitbox.Faction = character.Faction; // character is null before base
    }

    public override void Unequip(Character character)
    {
        base.Unequip(character);
    }

    public void EnableParry()
    {
        EnableParry(Time.GetTicksMsec());
    }

    /// <summary>
    /// Makes this melee weapon be able to parry and be parried.
    /// </summary>
    public void EnableParry(ulong parryTimeOrigin)
    {
        HasParried = false;
        IsParried = false;
        IsParryable = true;
        ParryTimeOrigin = parryTimeOrigin;
    }

    /// <summary>
    /// Makes this melee weapon be able to parry and be parried.
    /// </summary>
    public void DisableParry()
    {
        HasParried = false;
        IsParried = false;
        IsParryable = false;
    }

    public override void Use()
    {
        StateMachine.Use();
        base.Use();
    }

    public override void Deuse()
    {
        StateMachine.Deuse();
        base.Deuse();
    }

    public override void UseAlt()
    {
        StateMachine.UseAlt();
        base.UseAlt();
    }

    public override void DeuseAlt()
    {
        StateMachine.DeuseAlt();
        base.DeuseAlt();
    }

    /// <summary>
    /// Enables the weapon's hitbox. Prefer to call this from a state machine
    /// rather than managing state through the weapon script.
    /// </summary>
    public void Attack()
    {
        //RemainingAttackTime = AttackTime;
        IsAttacking = true;
        Hitbox.IsDisabled = false;
    }

    /// <summary>
    /// Disables the weapon's hitbox and processes all hurtboxes it hit.
    /// </summary>
    public void Deattack()
    {
        IsAttacking = false;
        Hitbox.IsDisabled = true;
        ProcessHits();
        DisableParry();
        Hitbox.ResetIgnoreList();
        AnimationPlayer.SpeedScale = 1;
    }

    public override void _Ready()
    {
        Hitbox.Damage = Damage;
        Hitbox.Hit += OnHitboxHit;
        StateMachine.StateChanged += OnStateChanged;
    }

    public void OnStateChanged(Node state)
    {
        if (StateMachine.UsedItemStates is null)
        {
            return;
        }

        foreach (var nodePath in StateMachine.UsedItemStates)
        {
            if (StateMachine.GetNode(nodePath) == state)
            {
                Character.Inventory.EmitSignal(
                    Inventory.SignalName.UsedItem, this);
                //EmitSignal(SignalName.UsedItem, this);
            }
        }
    }

    public override void _Process(double delta)
    {
        StateMachine.Process(delta);
        base._Process(delta);
    }

    /// <summary>
    /// Processes all hits and applies damages to hurtboxes.
    /// </summary>
    public void ProcessHits()
    {
        if (IsParried || HasParried)
        {
            return;
        }

        foreach (BoundingBox box in Hitbox.Hits)
        {
            if (box is Hurtbox hurtbox)
            {
                hurtbox.InflictDamage(
                    Damage,
                    Character,
                    Knockback,
                    this);
            }
        }

        if (Hitbox.Hits.Count > 0)
        {
            Character.ApplyImpulse(-Character.Target.Normalized() * Knockback);
        }
    }

    public void AttemptParry(Weapon otherWeapon)
    {
        if (otherWeapon.IsParryable &&
            otherWeapon is IParryable otherParryable)
        {
            if (ParryTimeOrigin < otherParryable.ParryTimeOrigin)
            {
                // our character was parried
                ParryParticles.CloneOnWorld<GpuParticles2D>().EmitOneShot();
                Stun(otherParryable.BlockForce);

                if (otherParryable is Sword sword)
                {
                    if (sword.StateMachine.CurrentState is SwordBlockState b)
                    {
                        b.HasBlocked = true;
                    }
                }
            }
            else
            {
                HasParried = true;
            }
        }
    }

    /// <summary>
    /// Stuns the wepaon holder. This is unique to swords and melee weapons
    /// if they can parry.
    /// </summary>
    public void Stun(float blockForce)
    {
        IsParried = true;
        Character.Stats.AddStaggerDamage(Damage * blockForce);

        // optionally play parry sound
        GetNode<AudioStreamPlayer2D>("ParrySound")?
            .OnWorld()
            .WithPitchDeviation(0.125f)
            .PlayOneShot();
    }

    public override void OnHitboxHit(BoundingBox box)
    {
        if (IsParried)
        {
            return;
        }

        if (box is Hitbox hb)
        {
            Weapon w = hb.GetAncestor<Weapon>();
            if (w is not null)
            {
                AttemptParry(w);
            }
        }

        if (box is Hurtbox hurt)
        {
            if (hurt.GetParent() is Character c)
            {
                var item = c.Inventory.SelectedItem;
                if (item is Weapon w)
                {
                    AttemptParry(w);
                }
            }
        }
    }
}
