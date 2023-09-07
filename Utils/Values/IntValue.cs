using Godot;

namespace SupaLidlGame;

public partial class IntValue : Node, IValue<int>
{
    [Signal]
    public delegate void ChangedEventHandler(int oldValue, int newValue);

    protected int _value = default;

    [Export]
    public int Value
    {
        get => _value;
        set
        {
            EmitSignal(SignalName.Changed, _value, value);
            _value = value;
        }
    }
}
