using Godot;

namespace SupaLidlGame.Extensions
{
    public static class NodeExtensions
    {
        /// <summary>
        /// Iterates through each ancestor until it finds an ancestor of type
        /// <c>T</c>
        /// </summary>
        public static T GetAncestor<T>(this Node node) where T : Node
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
    }
}
