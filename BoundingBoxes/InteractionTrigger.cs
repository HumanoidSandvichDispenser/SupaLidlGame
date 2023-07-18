using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.BoundingBoxes;

public partial class InteractionTrigger : Area2D
{
    [Signal]
    public delegate void InteractionEventHandler();

    [Signal]
    public delegate void TargetEventHandler();

    [Signal]
    public delegate void UntargetEventHandler();

    /// <summary>
    /// Invokes or triggers an interaction to occur.
    /// </summary>
    public void InvokeInteraction()
    {
        EmitSignal(SignalName.Interaction);
    }
}
