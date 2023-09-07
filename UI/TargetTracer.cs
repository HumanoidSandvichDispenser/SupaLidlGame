using Godot;

public partial class TargetTracer : Node2D
{
    private Line2D _line;

    private float Intensity
    {
        get => SelfModulate.A;
        set
        {
            SelfModulate = new Color(SelfModulate, value);
        }
    }

    public override void _Ready()
    {
        _line = GetNode<Line2D>("Line2D");
    }
}
