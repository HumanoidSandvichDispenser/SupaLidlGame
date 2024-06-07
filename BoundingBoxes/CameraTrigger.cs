using Godot;

namespace SupaLidlGame.BoundingBoxes;

public partial class CameraTrigger : Area2D
{
    [Export]
    public Node Camera { get; set; }

    [Export]
    public int EnablePriority { get; set; } = 2;

    [Export]
    public int DisablePriority { get; set; } = 0;

    public override void _Ready()
    {
        Connect(SignalName.BodyEntered, new Callable(this, nameof(OnBodyEntered)));
        Connect(SignalName.BodyExited, new Callable(this, nameof(OnBodyExited)));
    }

    private void OnBodyEntered(Node2D node)
    {
        Camera.Set("priority", EnablePriority);
        Camera.Set("follow_target", node);
    }

    private void OnBodyExited(Node2D node)
    {
        Camera.Set("priority", DisablePriority);
    }
}
