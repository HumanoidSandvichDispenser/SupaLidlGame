using Godot;

namespace SupaLidlGame.Debug;

public partial class Entry : LineEdit
{
    [Signal]
    public delegate void ConsoleInputEventHandler(string input);

    public override void _Ready()
    {
        GuiInput += OnGuiInput;
    }

    public void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            if (key.KeyLabel == Key.Enter && !key.Pressed)
            {
                EmitSignal(SignalName.ConsoleInput, Text);

                if (!key.CtrlPressed)
                {
                    Text = "";
                }
            }
        }
    }
}
