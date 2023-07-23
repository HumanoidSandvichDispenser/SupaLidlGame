using Godot;

namespace SupaLidlGame.BoundingBoxes;

public partial class CameraBounds : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        base._Ready();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Characters.Player player)
        {
            var camera = player.Camera;
            //camera.LimitLeft = 
        }
    }
}
