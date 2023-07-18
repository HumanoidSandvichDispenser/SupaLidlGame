using Godot;
using GodotUtilities;

namespace SupaLidlGame.Extensions;

public static class Node2DExtensions
{
    public static void RayCast(this Node2D node, Vector2 ray)
    {
        //var spaceState = node.GetWorld2d().DirectSpaceState;
        //var result = spaceState.IntersectRay();
    }

    public static Node CloneOnWorld(this Node2D node)
    {
        return CloneOnWorld<Node2D>(node);
    }

    public static T CloneOnWorld<T>(this Node2D node) where T : Node2D
    {
        var world = node.GetAncestor<Scenes.Map>();
        if (world is null)
        {
            throw new System.NullReferenceException("World does not exist");
        }

        //var parent = new Node2D();
        //world.AddChild(parent);
        //parent.GlobalPosition = node.GlobalPosition;
        var clone = node.Duplicate() as T;
        world.AddChild(clone);
        clone.GlobalPosition = node.GlobalPosition;
        return clone;
    }

    public static Node AtPosition(this Node2D node, Vector2 position)
    {
        node.GlobalPosition = position;
        return node;
    }
}
