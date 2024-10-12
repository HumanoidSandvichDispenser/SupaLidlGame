using Godot;
using SupaLidlGame.State.Character;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.State.Thinker;

public partial class DashDefensive : AttackState
{
    protected bool _dashedAway = false;
    protected State.Character.CharacterDashState _dashState;
    protected float _originalDashModifier;
    private Callable _dodgeCallable;

    [Export]
    public Area2D ProjectileDetection { get; set; }

    public override void _Ready()
    {
        _dashState = NPC.StateMachine.FindChildOfType<CharacterDashState>();
        _originalDashModifier = _dashState.VelocityModifier;
        _dodgeCallable = new Callable(this, MethodName.DodgeProjectile);

        base._Ready();
    }

    public override IState<ThinkerState> Enter(IState<ThinkerState> prev)
    {
        ProjectileDetection?.Connect(Area2D.SignalName.AreaEntered,
            _dodgeCallable);

        return base.Enter(prev);
    }

    public override void Exit(IState<ThinkerState> prev)
    {
        ProjectileDetection?.Disconnect(Area2D.SignalName.AreaEntered,
            _dodgeCallable);

        base.Exit(prev);
    }

    private void DodgeProjectile(Area2D area)
    {
        if (area is BoundingBoxes.Hitbox hitbox)
        {
            if (hitbox.GetOwner() is Entities.Projectile projectile)
            {
                GD.Print("changing direction");
                var direction = projectile.Direction;
                var dirToChar = projectile.GlobalPosition
                    .DirectionTo(NPC.GlobalPosition);
                var lateralDirection = Mathf.Sign(direction.Cross(dirToChar));
                DashTo(direction.Rotated(lateralDirection * Mathf.Pi / 2));
            }
        }
    }

    public override ThinkerState Think()
    {
        Characters.Character bestTarget = NPC.FindBestTarget();
        if (bestTarget is not null)
        {
            Vector2 pos = bestTarget.GlobalPosition;
            NPC.Target = pos - NPC.GlobalPosition;
            Vector2 dir = NPC.GlobalPosition.DirectionTo(pos);
            float dist = NPC.GlobalPosition.DistanceTo(pos);
            UpdateWeights(pos);

            if (dist > MaxDistanceToTarget)
            {
                if (PursueState is not null)
                {
                    return PursueState;
                }

                if (PassiveState is not null)
                {
                    return PassiveState;
                }
            }

            if (PursueOnNoLOS && !NPC.HasLineOfSight(bestTarget))
            {
                return PursueState;
            }

            if (NPC.CanAttack && NPC.StunTime <= 0)
            {
                bool isTargetStunned = bestTarget.StunTime > 0;

                bool shouldDashAway = false;
                bool shouldDashTowards = false;

                var currentItem = NPC.Inventory.SelectedItem;
                if (currentItem is not Items.Weapons.Sword sword)
                {
                    return null;
                }

                var swordState = sword.StateMachine.CurrentState;

                float dot = NPC.Direction.Normalized()
                    .Dot(bestTarget.Direction.Normalized());

                // doc will still dash if you are farther than normal but
                // moving towards him
                float distThreshold = UseItemDistance - (dot * 20);

                // dash towards if lance in anticipate state
                // or just directly dash towards you if you are too far
                shouldDashTowards = (isTargetStunned || _dashedAway) &&
                    (swordState is State.Weapon.SwordAnticipateState ||
                    dist > MaxDistanceToTarget);

                shouldDashAway = dist < distThreshold && !isTargetStunned &&
                    swordState is State.Weapon.SwordIdleState;

                //if (!isTargetStunned && dist < 2500 && !_dashedAway)
                if (shouldDashAway && !shouldDashTowards)
                {
                    // dash away if too close
                    _dashState.VelocityModifier = _originalDashModifier;
                    DashTo(-dir);
                    NPC.UseCurrentItem();
                    _dashedAway = true;
                }
                else if (shouldDashTowards && !shouldDashAway)
                {
                    // our required velocity is dependent on final distance to target
                    float maxVelocity = dist / 0.1f;
                    var dashSpeed = Mathf.Max(maxVelocity,
                        _originalDashModifier * NPC.Speed);
                    float ratio = dashSpeed / NPC.Speed;
                    _dashState.VelocityModifier = ratio;

                    // dash to player's predicted position
                    var newPos = Utils.Physics.PredictNewPosition(
                        NPC.GlobalPosition,
                        dashSpeed,
                        pos,
                        bestTarget.Velocity,
                        out float _);
                    DashTo(NPC.GlobalPosition.DirectionTo(newPos));
                    _dashedAway = false;
                }
                else if (isTargetStunned)
                {
                    NPC.UseCurrentItem();
                }
            }

            return null;
        }

        return PursueState ?? PassiveState;
    }

    private void DashTo(Vector2 direction)
    {
        var stateMachine = NPC.StateMachine;
        stateMachine.ChangeState<CharacterDashState>(out var state);
        state.DashDirection = direction;
    }
}
