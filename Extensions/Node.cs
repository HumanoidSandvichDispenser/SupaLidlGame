using Godot;

namespace SupaLidlGame.Extensions;

public static class NodeExtensions
{
    /// <summary>
    /// Iterates through each ancestor until it finds an ancestor of type
    /// <c>T</c>
    /// </summary>
    [System.Obsolete]
    public static T GetAncestorDeprecated<T>(this Node node) where T : Node
    {
        Node parent;

        while ((parent = node.GetParent()) != null)
        {
            if (parent is T t)
            {
                return t;
            }

            node = parent;
        }

        return null;
    }

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
}
