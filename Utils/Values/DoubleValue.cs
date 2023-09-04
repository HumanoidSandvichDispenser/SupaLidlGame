using Godot;

namespace SupaLidlGame;

public partial class DoubleValue : Node, IValue<double>
{
    [Signal]
    public delegate void ChangedEventHandler(double oldValue, double newValue);

    protected double _value = default;

    [Export]
    public double Value
    {
        get => _value;
        set
        {
            EmitSignal(SignalName.Changed, _value, value);
            _value = value;
        }
    }
}
