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

    private Control _popup;

    public override void _Ready()
    {
        base._Ready();

        _popup = GetNode<Control>("Popup");
        _popup.Visible = false;
    }

    /// <summary>
    /// Invokes or triggers an interaction to occur.
    /// </summary>
    public void InvokeInteraction()
    {
        EmitSignal(SignalName.Interaction);
    }

    public void Focus()
    {
        _popup.Visible = true;
    }

    public void Unfocus()
    {
        _popup.Visible = false;
    }
}
