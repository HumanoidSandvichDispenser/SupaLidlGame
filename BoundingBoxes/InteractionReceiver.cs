using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.BoundingBoxes;

public partial class InteractionReceiver : Area2D
{
    [Signal]
    public delegate void TriggerEventHandler(
        InteractionTrigger trigger,
        InteractionTrigger closestTrigger);

    [Signal]
    public delegate void ClosestTriggerEventHandler(InteractionTrigger trigger);

    private InteractionTrigger _closestTrigger;

    private HashSet<InteractionTrigger> _triggers;

    public InteractionReceiver()
    {
        _triggers = new HashSet<InteractionTrigger>();
    }

    private void UpdateClosestTrigger()
    {
        float minDist = float.MaxValue;
        InteractionTrigger best = null;

        foreach (var trigger in _triggers)
        {
            float dist = trigger.GlobalPosition
                .DistanceSquaredTo(GlobalPosition);

            if (dist <= minDist)
            {
                best = trigger;
                minDist = dist;
            }
        }

        if (_closestTrigger != best)
        {
            EmitSignal(SignalName.ClosestTrigger, best);
            _closestTrigger = best;
        }
    }

    public void _on_area_entered(Area2D area)
    {
        if (area is InteractionTrigger trigger)
        {
            EmitSignal(SignalName.Trigger, _closestTrigger);
        }
    }

    public void _on_area_exited(Area2D area)
    {
        // update closest trigger
        if (area is InteractionTrigger trigger)
        {
            if (_triggers.Contains(trigger))
            {
                _triggers.Remove(trigger);
            }
            UpdateClosestTrigger();
        }
        GD.PushWarning("Area entered is not an InteractionTrigger.");
    }
}
