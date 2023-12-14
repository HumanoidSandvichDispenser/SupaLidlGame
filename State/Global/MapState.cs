using Godot;
using Godot.Collections;

namespace SupaLidlGame.State.Global;

[GlobalClass]
public partial class MapState : Resource
{
    [Export]
    private Dictionary<string, Variant> _state;

    [Signal]
    public delegate void MapStateChangedEventHandler(string key, Variant value);

    [Signal]
    public delegate void MapStateBoolChangedEventHandler(string key, bool value);

    public Variant this[string key]
    {
        get
        {
            if (_state.ContainsKey(key))
            {
                return _state[key];
            }
            return default;
        }
        set
        {
            if (_state.ContainsKey(key))
            {
                _state[key] = value;
            }
            else
            {
                _state.Add(key, value);
            }
            EmitSignal(SignalName.MapStateChanged, key, value);

            switch (value.VariantType)
            {
                case Variant.Type.Bool:
                    EmitSignal(SignalName.MapStateBoolChanged, key, (bool)value);
                    break;
            }
        }
    }
}
