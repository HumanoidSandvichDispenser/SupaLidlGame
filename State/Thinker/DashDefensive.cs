using Godot;
using SupaLidlGame.State.Character;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.State.Thinker;

public partial class DashDefensive : AttackState
{
    protected bool _dashedAway = false;
    protected State.Character.CharacterDashState _dashState;
    protected float _originalDashModifier;

    public override void _Ready()
    {
        _dashState = NPC.StateMachine.FindChildOfType<CharacterDashState>();
        _originalDashModifier = _dashState.VelocityModifier;
        base._Ready();
    }

    public override ThinkerState Think()
    {
        Characters.Character bestTarget = NPC.FindBestTarget();
        if (bestTarget is not null)
        {
            Vector2 pos = bestTarget.GlobalPosition;
            NPC.Target = pos - NPC.GlobalPosition;
            Vector2 dir = NPC.GlobalPosition.DirectionTo(pos);
            float dist = NPC.GlobalPosition.DistanceSquaredTo(pos);
            UpdateWeights(pos);

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
                float distThreshold = 2500 - (dot * 400);

                // or just directly dash towards you if you are too far
                float distTowardsThreshold = 22500;

                // dash towards if lance in anticipate state
                shouldDashTowards = (isTargetStunned || _dashedAway) &&
                    swordState is State.Weapon.SwordAnticipateState ||
                    dist > distTowardsThreshold;

                shouldDashAway = dist < distThreshold && !isTargetStunned;

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
                    // dash to player's predicted position
                    _dashState.VelocityModifier = _originalDashModifier * 1.75f;
                    var dashSpeed = _dashState.VelocityModifier * NPC.Speed;
                    var newPos = Utils.Physics.PredictNewPosition(
                        NPC.GlobalPosition,
                        dashSpeed,
                        pos,
                        bestTarget.Velocity,
                        out float _);
                    DashTo(NPC.GlobalPosition.DirectionTo(newPos));
                    _dashedAway = false;
                }
            }
        }

        return null;
    }

    private void DashTo(Vector2 direction)
    {
        var stateMachine = NPC.StateMachine;
        stateMachine.ChangeState<CharacterDashState>(out var state);
        state.DashDirection = direction;
    }
}
