using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.Debug;

internal static class NodePathParser
{
    internal static IEnumerable<NodePathToken> ParseNodePath(CharIterator iterator)
    {
        // Some/Node/Path:And:Property/More/Paths
        // ->
        // Some/Node/Path (Node)
        // :And:Property  (Property)
        // More/Paths     (Node)

        NodePathTokenType curType = NodePathTokenType.Node;
        string path = "";
        while (iterator.GetNext() != '\0')
        {
            char curChar = iterator.MoveNext();

            if (curChar == ':')
            {
                // if we have been parsing a nodepath, yield a nodepath
                if (curType == NodePathTokenType.Node)
                {
                    if (path.Length > 0)
                    {
                        yield return new NodePathToken(path, curType);
                        path = "";
                        curType = NodePathTokenType.Property;
                    }
                }
                else
                {
                    path += curChar;
                }
            }
            else if (curChar == '/')
            {
                // if we have been parsing property, yield a property
                if (curType == NodePathTokenType.Property)
                {
                    yield return new NodePathToken(path, curType);
                    path = "";
                    curType = NodePathTokenType.Node;
                }
                else
                {
                    path += curChar;
                }
            }
            else
            {
                path += curChar;
            }
        }

        // reached the end
        if (path.Length > 0)
        {
            yield return new NodePathToken(path, curType);
        }
    }
}
