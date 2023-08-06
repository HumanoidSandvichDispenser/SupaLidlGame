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

                if (value is not null)
                {
                    // focus on the new trigger
                    value.Focus();
                }
            }

            if (_trigger is not null)
            {
                if (_trigger != value)
                {
                    // unfocus from the old trigger
                    _trigger.Unfocus();
                }
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
