using Godot;
using GodotUtilities;
using SupaLidlGame.Extensions;
using SupaLidlGame.State.Character;

namespace SupaLidlGame.Characters;

public partial class Doc : Boss
{
    public AnimationPlayer TelegraphAnimation { get; set; }

    [Export]
    public Items.Weapons.Sword Lance { get; set; }

    protected bool _dashedAway = false;
    protected CharacterDashState _dashState;
    protected float _originalDashModifier;

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
                case < 300:
                    return 3;
                case < 600:
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

        _dashState = StateMachine.FindChildOfType<CharacterDashState>();
        _originalDashModifier = _dashState.VelocityModifier;


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

                bool shouldDashAway = false;
                bool shouldDashTowards = false;

                var lance = Inventory.SelectedItem as Items.Weapons.Sword;
                var lanceState = lance.StateMachine.CurrentState;

                float dot = Direction.Normalized()
                    .Dot(bestTarget.Direction.Normalized());

                // doc will still dash if you are farther than normal but
                // moving towards him
                float distThreshold = 2500 - (dot * 400);

                // or just directly dash towards you if you are too far
                float distTowardsThreshold = 22500;

                // dash towards if lance in anticipate state
                shouldDashTowards = (isTargetStunned || _dashedAway) &&
                    lanceState is State.Weapon.SwordAnticipateState ||
                    dist > distTowardsThreshold;

                shouldDashAway = dist < distThreshold && !isTargetStunned;

                //if (!isTargetStunned && dist < 2500 && !_dashedAway)
                if (shouldDashAway && !shouldDashTowards)
                {
                    // dash away if too close
                    _dashState.VelocityModifier = _originalDashModifier;
                    DashTo(-dir);
                    UseCurrentItem();
                    _dashedAway = true;
                }
                else if (shouldDashTowards && !shouldDashAway)
                {
                    // dash to player's predicted position
                    _dashState.VelocityModifier = _originalDashModifier * 1.75f;
                    var dashSpeed = _dashState.VelocityModifier * Speed;
                    var newPos = Utils.Physics.PredictNewPosition(
                        GlobalPosition,
                        dashSpeed,
                        pos,
                        bestTarget.Velocity,
                        out float _);
                    DashTo(GlobalPosition.DirectionTo(newPos));
                    _dashedAway = false;
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
