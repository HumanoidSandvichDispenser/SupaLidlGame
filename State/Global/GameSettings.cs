using Godot;

namespace SupaLidlGame.State.Global;

public enum InputMethod
{
    Mouse,
    Joystick,
    MouseCentered,
}

[GlobalClass]
public partial class GameSettings : Resource
{
    [Export]
    public InputMethod InputMethod { get; set; }
}
