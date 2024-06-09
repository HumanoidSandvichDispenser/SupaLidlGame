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

    public override void _EnterTree()
    {
        if (!Camera.Get("tween_on_load").AsBool())
        {
            var tween = Camera.Get("tween_resource").AsGodotObject();
            double duration = tween.Get("duration").AsDouble();

            // HACK: manually set this field to interrupt tween when the
            // trigger enters the scene, which happens when the scene is loaded
            // from the cache
            Camera.SetDeferred("_has_tweened", true);
            GD.Print("set tween");
        }
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
