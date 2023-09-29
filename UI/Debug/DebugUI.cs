using SupaLidlGame.Debug;
using Godot;

namespace SupaLidlGame.UI.Debug;

public partial class DebugUI : CanvasLayer
{
    private bool _areWindowsVisible = false;
    private Window _lastFocusedWindow = null;

    public bool AreWindowsVisible
    {
        get => _areWindowsVisible;
        set
        {
            _areWindowsVisible = value;
            foreach (var node in GetChildren())
            {
                if (node is Window w)
                {
                    w.Visible = value;
                }
                else if (node is Control c)
                {
                    c.Visible = value;
                }
            }
        }
    }

    //public override void _Ready()
    //{
    //    AreWindowsVisible = false;

    //    ChildEnteredTree += (Node child) =>
    //    {
    //        if (child is Window w)
    //        {
    //            void setFocus()
    //            {
    //                _lastFocusedWindow = w;
    //            }
    //            w.FocusEntered += setFocus;
    //        }
    //    };

    //    _lastFocusedWindow = GetNode<Window>("%ConsoleWindow");
    //}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            if (key.Keycode == Key.Quoteleft)
            {
                if (key.Pressed)
                {
                    Toggle();
                }
            }
        }
    }

    public void Toggle()
    {
        AreWindowsVisible = !AreWindowsVisible;
    }
}
