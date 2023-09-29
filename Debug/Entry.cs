using Godot;
using GodotUtilities;
using System.Linq;

namespace SupaLidlGame.Debug;

public partial class Entry : CodeEdit
{
    [Signal]
    public delegate void ConsoleInputEventHandler(string input);

    public override void _Ready()
    {
        GuiInput += OnGuiInput;

        AddStringDelimiter("\'", "\"", true);
        AddStringDelimiter("'", "'", true);
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
        if (@event is InputEventKey key && key.Pressed)
        {
            if (key.KeyLabel == Key.Enter)
            {
                AcceptEvent();
                if (key.Pressed)
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

    public void SetContext(Node context)
    {
        var properties = context.GetPropertyList();
        var propNames = properties.Select((prop) => prop["name"].ToString());
        foreach (var prop in propNames)
        {
            AddCodeCompletionOption(
                CodeCompletionKind.Member,
                prop, prop);
        }
        UpdateCodeCompletionOptions(true);
    }
}
