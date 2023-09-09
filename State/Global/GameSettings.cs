using Godot;

namespace SupaLidlGame.State.Global;

public enum InputMethod
{
    Mouse,
    Joystick,
    MouseCentered,
}

public partial class GameSettings : Resource
{
    [Export]
    public InputMethod InputMethod { get; set; }
}
