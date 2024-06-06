using Godot;

namespace SupaLidlGame.BoundingBoxes;

public partial class CameraBounds : Node2D
{
    [Export]
    public Area2D Trigger { get; set; }

    //[Export]
    //public Rect2I Bounds { get; set; }

    [Export]
    public Marker2D TopLeft { get; set; }

    [Export]
    public Marker2D BottomRight { get; set; }

    public override void _Ready()
    {
        //Trigger.BodyEntered += OnBodyEntered;
        //Trigger.BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Characters.Player player)
        {
            var camera = player.Camera;
            var rect = new Rect2I();
            rect.Position = (Vector2I)TopLeft.GlobalPosition;
            rect.End = (Vector2I)BottomRight.GlobalPosition;
            camera.SetCameraBounds(rect);
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is Characters.Player player)
        {
            var camera = player.Camera;
            camera.LimitLeft = int.MinValue;
            camera.LimitTop = int.MinValue;
            camera.LimitRight = int.MaxValue;
            camera.LimitBottom = int.MaxValue;
        }
    }
}
