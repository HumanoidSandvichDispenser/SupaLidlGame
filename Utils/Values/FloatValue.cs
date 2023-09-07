using Godot;

namespace SupaLidlGame;

public partial class FloatValue : Node, IValue<float>
{
    [Signal]
    public delegate void ChangedEventHandler(float oldValue, float newValue);

    protected float _value = default;

    [Export]
    public float Value
    {
        get => _value;
        set
        {
            EmitSignal(SignalName.Changed, _value, value);
            _value = value;
        }
    }
}
