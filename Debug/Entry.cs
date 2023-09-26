using Godot;

namespace SupaLidlGame.Debug;

public partial class Entry : CodeEdit
{
    [Signal]
    public delegate void ConsoleInputEventHandler(string input);

    public override void _Ready()
    {
        GuiInput += OnGuiInput;
    }

    /*
    public override void _Input(InputEvent @event)
    {
        if (HasFocus())
        {
            if (@event is InputEventKey && @event.IsPressed())
            {
                AcceptEvent();
                OnGuiInput(@event);
            }
        }
    }
    */

    public void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            if (key.KeyLabel == Key.Enter)
            {
                AcceptEvent();
                if (!key.Pressed)
                {
                    EmitSignal(SignalName.ConsoleInput, Text);

                    if (!key.IsCommandOrControlPressed())
                    {
                        Text = "";
                    }
                }
            }
        }
    }
}
