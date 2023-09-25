using Godot;

public enum NodePathTokenType
{
    Node,
    Property
}

public struct NodePathToken
{
    public NodePath Path { get; set; }

    public NodePathTokenType Type { get; set; }

    public NodePathToken(NodePath path, NodePathTokenType type)
    {
        Path = path;
        Type = type;
    }
}
