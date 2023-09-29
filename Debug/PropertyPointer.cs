using Godot;

namespace SupaLidlGame.Debug;

public struct PropertyPointer
{
    public Node Node { get; set; }

    public string Property { get; set; }

    public PropertyPointer(Node node, string property)
    {
        Node = node;
        Property = property;
    }

    public Variant Dereferenced => Node.Get(Property);

    public void Set(Variant value) => Node.Set(Property, value);
}
