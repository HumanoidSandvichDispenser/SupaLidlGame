using Godot;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Characters;
using SupaLidlGame.Extensions;
using SupaLidlGame.State.Weapon;

namespace SupaLidlGame.Items.Weapons;

public partial class Sword : Weapon, IParryable
{
    public bool IsAttacking { get; protected set; }

    public override bool IsUsing => StateMachine.CurrentState
        is SwordAttackState;

    [Export]
    public Hitbox Hitbox { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayer { get; set; }

    [Export]
    public AnimationTree AnimationTree { get; set; }

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
    public double AttackAnimationDuration { get; set; }

    [Export]
    public CpuParticles2D ParryParticles { get; set; }

    [Export]
    public double NPCAnticipateTime { get; set; }

    [Export]
    public WeaponStateMachine StateMachine { get; set; }

    [Export]
    public Node2D Anchor { get; set; }

    public override bool IsParryable { get; protected set; }

    public ulong ParryTimeOrigin { get; protected set; }

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

    public override void Use()
    {
        // we can't use if we're still using the weapon
        if (RemainingUseTime > 0)
        {
            //return;
        }

        StateMachine.Use();

        /*
        // reset state of the weapon
        IsParried = false;
        IsParryable = true;
        ParryTimeOrigin = Time.GetTicksMsec();

        _playback.Travel("use");
        */

        // play animation depending on rotation of weapon
        /*
        string anim = "use";

        if (GetNode<Node2D>("Anchor").Rotation > Mathf.DegToRad(50))
        {
            anim = "use2";
        }

        if (Character is NPC)
        {
            // NPCs have a slower attack
            anim += "-npc";
        }

        AnimationPlayer.Play(anim);
        */

        base.Use();
    }

    public void EnableParry()
    {
        IsParried = false;
        IsParryable = true;
        ParryTimeOrigin = Time.GetTicksMsec();
        GD.Print(Character.Name);
    }

    public void DisableParry()
    {
        IsParryable = false;
    }

    public override void Deuse()
    {
        //AnimationPlayer.Stop();
        Deattack();
        base.Deuse();
    }

    public void Attack()
    {
        //RemainingAttackTime = AttackTime;
        IsAttacking = true;
        Hitbox.IsDisabled = false;
    }

    public void Deattack()
    {
        IsAttacking = false;
        DisableParry();
        Hitbox.IsDisabled = true;
        ProcessHits();
        Hitbox.ResetIgnoreList();
        AnimationPlayer.SpeedScale = 1;
    }

    public override void _Ready()
    {
        Hitbox.Damage = Damage;
        _playback = (AnimationNodeStateMachinePlayback)AnimationTree
            .Get("parameters/playback");
    }

    public override void _Process(double delta)
    {
        StateMachine.Process(delta);
        base._Process(delta);
    }

    public void ProcessHits()
    {
        if (IsParried)
        {
            return;
        }

        foreach (BoundingBox box in Hitbox.Hits)
        {
            GD.Print("processing hit");
            if (box is Hurtbox hurtbox)
            {
                hurtbox.InflictDamage(Damage, Character, Knockback);
            }
        }
    }

    public void AttemptParry(Weapon otherWeapon)
    {
        //if (IsParryable && otherWeapon.IsParryable)
        if (otherWeapon.IsParryable &&
                otherWeapon is IParryable otherParryable)
        {
            ParryParticles.Emitting = true;
            if (ParryTimeOrigin < otherParryable.ParryTimeOrigin)
            {
                // our character was parried
            }
            else
            {
                otherParryable.Stun();
            }
        }
        //this.GetAncestor<TileMap>().AddChild(instance);
    }

    public void Stun()
    {
        IsParried = true;
        AnimationPlayer.SpeedScale = 0.25f;
        Character.Stun(1.5f);
        GetNode<AudioStreamPlayer2D>("ParrySound").OnWorld().Play();
    }

    public override void _on_hitbox_hit(BoundingBox box)
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

    protected void SetAnimationCondition(string condition, bool value)
    {
        AnimationTree.Set("parameters/conditions/" + condition, value);
    }
}
