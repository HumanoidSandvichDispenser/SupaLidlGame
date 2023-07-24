using Godot;
using GodotUtilities;
using SupaLidlGame.State.Character;

namespace SupaLidlGame.Characters;

public partial class Doc : Boss
{
    public AnimationPlayer TelegraphAnimation { get; set; }

    [Export]
    public Items.Weapons.Sword Lance { get; set; }

    protected bool _dashedAway = false;

    public override float Health
    {
        get => base.Health;
        set
        {
            if (IsActive)
            {
                base.Health = value;
            }
            else
            {
                // play opening animation
                // then become active when it finishes
                base.Health = value;
            }
        }
    }

    public override int Intensity
    {
        get
        {
            switch (Health)
            {
                case < 200:
                    return 3;
                case < 400:
                    return 2;
                default:
                    return 1;
            }
        }
    }

    public Doc()
    {
        ShouldMove = false;
    }

    public override void _Ready()
    {
        TelegraphAnimation = GetNode<AnimationPlayer>("Animations/Telegraph");
        base._Ready();

        // when we are hurt, start the boss fight
        Hurt += (Events.HealthChangedArgs args) =>
        {
            if (!IsActive)
            {
                IsActive = true;
                Inventory.SelectedItem = Lance;
            }
        };
    }
    
    public override void _Process(double delta)
    {
        if (IsActive)
        {
            BossStateMachine.Process(delta);
        }
        base._Process(delta);
    }

    protected override float ReceiveDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackDir = default)
    {
        if (IsActive)
        {
            return base.ReceiveDamage(
                damage, inflictor, knockback, knockbackDir);
        }

        return 1;
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

    protected override void Think()
    {
        if (BossStateMachine.CurrentState is State.NPC.Doc.DocLanceState)
        {
            ThirdPhaseThink();
        }
        else
        {
            base.Think();
        }
    }

    protected void ThirdPhaseThink()
    {
        Character bestTarget = FindBestTarget();
        if (bestTarget is not null)
        {
            Vector2 pos = bestTarget.GlobalPosition;
            Target = pos - GlobalPosition;
            Vector2 dir = GlobalPosition.DirectionTo(pos);
            float dist = GlobalPosition.DistanceSquaredTo(pos);
            UpdateWeights(pos);

            if (CanAttack && StunTime <= 0)
            {
                bool isTargetStunned = bestTarget.StunTime > 0;
                if (!isTargetStunned && dist < 2500)
                {
                    if (Inventory.SelectedItem is Items.Weapon weapon)
                    {
                        // dash away if too close
                        DashTo(-dir);
                        UseCurrentItem();
                        _dashedAway = true;
                    }
                }
                else if (isTargetStunned || (dist < 3600 && _dashedAway))
                {
                    if (!Inventory.SelectedItem.IsUsing)
                    {
                        DashTo(dir);
                        UseCurrentItem();
                        _dashedAway = false;
                    }
                }
            }
        }
    }

    private void DashTo(Vector2 direction)
    {
        StateMachine.ChangeState<CharacterDashState>(out var state);
        state.DashDirection = direction;
    }
}
