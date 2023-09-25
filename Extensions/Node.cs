using Godot;

namespace SupaLidlGame.Extensions;

public static class NodeExtensions
{
    /// <summary>
    /// A version <c>GetNode</c> that returns null rather than cause an
    /// exception if the node is not found or is not the same type.
    /// </summary>
    /// <returns>
    /// <see langword="null">null</see> if <param>name</param> does not match
    /// a valid Node
    /// </returns>
    public static T GetN<T>(this Node node, string name) where T : Node
    {
        return node.GetNode(name) as T;
    }

    public static T FindChildOfType<T>(this Node node) where T : Node
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is T t)
            {
                return t;
            }
        }

        return null;
    }

    public static State.Global.GlobalState GetGlobalState(this Node node)
    {
        return node.GetNode<State.Global.GlobalState>("/root/GlobalState");
    }

    public static Events.EventBus GetEventBus(this Node node)
    {
        return node.GetNode<Events.EventBus>("/root/EventBus");
    }

    public static Utils.World GetWorld(this Node node)
    {
        return node.GetNode<Utils.World>("/root/World");
    }

    public static CanvasLayer GetUI(this Node node)
    {
        return node.GetNode<CanvasLayer>("/root/BaseUI");
    }

    public static UI.UIController GetMainUI(this Node node)
    {
        return node.GetNode<UI.UIController>("/root/BaseUI/" +
            "SubViewportContainer/UIViewport/CanvasLayer/MainUILayer/Main");
    }
}
