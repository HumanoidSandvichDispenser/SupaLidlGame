using Godot;

namespace SupaLidlGame.Utils;

public partial class Trail : Line2D
{
    [Export]
    public int MaximumPoints { get; set; }

    [Export]
    protected uint Frequency { get; set; }

    public bool IsDead { get; set; } = false;

    protected double _currentTrailSleepTime = 0;
    protected double _trailSleepTime = 0;
    protected Node2D _parent;

    public override void _Ready()
    {
        _trailSleepTime = 1.0 / Frequency;
        _parent = GetParent() as Node2D;
    }

    public override void _Process(double delta)
    {
        if (IsDead && Points.Length > 0)
        {
            RemovePoint(0);
            return;
        }

        //Vector2 point = GlobalPosition;
        if ((_currentTrailSleepTime -= delta) > 0)
        {
            if (Points.Length > 0)
            {
                SetPointPosition(Points.Length - 1, _parent.GlobalPosition);
            }
            return;
        }

        _currentTrailSleepTime = _trailSleepTime;

        AddPoint(_parent.GlobalPosition);

        while (Points.Length > MaximumPoints)
        {
            RemovePoint(0);
        }
    }
}
