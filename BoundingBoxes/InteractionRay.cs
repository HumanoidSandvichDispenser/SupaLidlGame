using Godot;

namespace SupaLidlGame.BoundingBoxes;

public partial class InteractionRay : RayCast2D
{
    private InteractionTrigger _trigger;

    public InteractionTrigger Trigger
    {
        get => _trigger;
        protected set
        {
            if (_trigger != value)
            {
                EmitSignal(SignalName.TriggerHit, value);
            }

            if (_trigger is not null)
            {
                LastValidTrigger = value;
            }

            _trigger = value;
        }
    }

    public InteractionTrigger LastValidTrigger { get; set; }

    [Signal]
    public delegate void TriggerHitEventHandler(InteractionTrigger trigger);

    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsColliding())
        {
            var obj = GetCollider();
            // if obj is an InteractionTrigger then we signal hit
            if (obj is InteractionTrigger trigger)
            {
                Trigger = trigger;
            }
        }
        else
        {
            Trigger = null;
        }
    }
}
