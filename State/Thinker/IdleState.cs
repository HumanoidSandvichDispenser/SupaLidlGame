using Godot;

namespace SupaLidlGame.State.Thinker;

public partial class IdleState : ThinkerState
{
    [Export]
    public ThinkerState PursueState { get; set; }

    [Export]
    public float MinTargetDistance { get; set; }

    public override ThinkerState Think()
    {
        var bestTarget = NPC.FindBestTarget();
        if (bestTarget is not null)
        {
            var pos = bestTarget.GlobalPosition;
            var dist = NPC.GlobalPosition.DistanceTo(pos);
            if (dist < MinTargetDistance)
            {
                return PursueState;
            }
        }

        return base.Think();
    }
}
