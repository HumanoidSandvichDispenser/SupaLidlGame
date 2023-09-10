using Godot;

public partial class TargetTracer : Node2D
{
    private Line2D _line;

    private float _intensity = 127;

    public float Intensity
    {
        get => _intensity;
        set
        {
            var color = new Color(Colors.White, value);
            (Material as ShaderMaterial)
                .SetShaderParameter("tint_color", color);
            _intensity = value;
        }
    }

    public override void _Ready()
    {
        _line = GetNode<Line2D>("Line2D");
    }
}
