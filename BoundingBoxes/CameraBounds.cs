using Godot;

namespace SupaLidlGame.BoundingBoxes;

public partial class CameraBounds : Node
{
    [Export]
    public Area2D Trigger { get; set; }

    [Export]
    public RectangleShape2D Bounds { get; set; }

    public override void _Ready()
    {
        Trigger.BodyEntered += OnBodyEntered;
        Trigger.BodyExited += OnBodyExited;
        base._Ready();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Characters.Player player)
        {
            var camera = player.Camera;
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is Characters.Player player)
        {
            var camera = player.Camera;
            camera.LimitLeft = -1024;
        }
    }
}
