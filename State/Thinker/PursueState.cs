using Godot;

namespace SupaLidlGame.State.Thinker;

public partial class PursueState : ThinkerState
{
    [Export]
    public NavigationAgent2D NavigationAgent { get; set; }

    [Export]
    public ThinkerState AttackState { get; set; }

    [Export]
    public ThinkerState PassiveState { get; set; }

    /// <summary>
    /// Minimum distance the NPC should be, otherwise entering attack state
    /// </summary>
    [Export]
    public float MinDistanceToTarget { get; set; }

    /// <summary>
    /// Maximum distance the NPC should be, otherwise entering passive state
    /// </summary>
    [Export]
    public float MaxDistanceFromOrigin { get; set; }

    public override ThinkerState Think()
    {
        var bestTarget = NPC.FindBestTarget();
        if (bestTarget is not null && NPC.HasLineOfSight(bestTarget))
        {
            // navigate towards the best target
            var pos = bestTarget.GlobalPosition;
            NavigationAgent.TargetPosition = pos;
            NPC.Target = NPC.GlobalPosition.DirectionTo(pos);
            NPC.LastSeenPosition = pos;

            if (NPC.GlobalPosition.DistanceTo(pos) < MinDistanceToTarget)
            {
                return AttackState;
            }
        }
        else
        {
            // go to last seen position of last best target
            NavigationAgent.TargetPosition = NPC.LastSeenPosition;
        }
        return null;
    }

    public override ThinkerState PhysicsProcess(double delta)
    {
        if (!NavigationAgent.IsTargetReachable())
        {
            return PassiveState ?? base.PhysicsProcess(delta);
        }

        var navPos = NavigationAgent.GetNextPathPosition();
        NPC.Direction = NPC.GlobalPosition.DirectionTo(navPos);

        return base.PhysicsProcess(delta);
    }
}
