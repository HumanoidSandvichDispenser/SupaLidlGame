using Godot;
using GodotUtilities;

namespace SupaLidlGame.State.Thinker;

public partial class IdleState : ThinkerState
{
    [Export]
    public ThinkerState PursueState { get; set; }

    [Export]
    public float MinTargetDistance { get; set; }

    [ExportGroup("Line of Sight")]
    [Export]
    public bool PursueOnLineOfSight { get; set; }

    [Export]
    public float MinLineOfSightDistance { get; set; }

    [ExportGroup("Returning")]
    [Export]
    public bool ShouldReturnToOriginalPosition { get; set; } = true;

    [Export]
    public NavigationAgent2D NavigationAgent { get; set; }

    protected Vector2 _originalPosition;
    protected bool _isReturning;

    public override void _Ready()
    {
        _originalPosition = NPC.GlobalPosition;
        base._Ready();
    }

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
            else if (PursueOnLineOfSight && NPC.HasLineOfSight(bestTarget))
            {
                if (dist < MinLineOfSightDistance)
                {
                    return PursueState;
                }
            }
            _isReturning = false;
        }

        if (ShouldReturnToOriginalPosition)
        {
            if (NavigationAgent is not null)
            {
                // if we're less than 8 units away then we don't move
                var pos = NPC.GlobalPosition;
                if (!pos.IsWithinDistanceSquared(_originalPosition, 64))
                {
                    NavigationAgent.TargetPosition = _originalPosition;
                }
            }
            _isReturning = true;
        }

        return base.Think();
    }

    public override ThinkerState PhysicsProcess(double delta)
    {
        if (_isReturning && (NavigationAgent?.IsTargetReachable() ?? false))
        {
            var navPos = NavigationAgent.GetNextPathPosition();
            NPC.Direction = NPC.GlobalPosition.DirectionTo(navPos);
        }

        if (NavigationAgent?.IsTargetReached() ?? false)
        {
            NPC.Direction = Vector2.Zero;
        }

        return base.PhysicsProcess(delta);
    }
}
