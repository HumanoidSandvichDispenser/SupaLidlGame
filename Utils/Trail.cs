using Godot;

public partial class Trail : Line2D
{
    [Export]
    public int MaximumPoints { get; set; }

    [Export]
    public Node2D Tracking { get; set; }

    public override void _Process(double delta)
    {
        Vector2 point = Tracking.Position;
        AddPoint(point);

        while (Points.Length > MaximumPoints)
        {
            RemovePoint(0);
        }
    }
}
