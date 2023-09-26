using System.Text.RegularExpressions;

namespace SupaLidlGame.Debug;

public static class Sanitizer
{
    private static Regex _nonAlphanum = new("[^a-zA-Z0-9_]");

    private static Regex _nonNodeName = new("[^a-zA-Z0-9_\\-\\/\\.\\:]");

    private static string ScanString(CharIterator iterator)
    {
        string ret = "";

        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();

            if (c == '"')
            {
                return ret;
            }
            else if (c == '\\')
            {
                char escape = iterator.MoveNext();

                switch (escape)
                {
                    case 'n':
                        ret += '\n';
                        break;
                    case 't':
                        ret += '\t';
                        break;
                    case '\0':
                        throw new InterpreterException("Unexpected EOL",
                            iterator.Line, iterator.Column); default:
                        ret += escape;
                        break;
                }
            }
            
            ret += c;
        }
        throw new InterpreterException("Unexpected EOL, expected '\"'",
            iterator.Line, iterator.Column);
    }

    private static string ScanNodePath(CharIterator iterator)
    {
        string ret = "";
        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();

            if (c == '"')
            {
                return ScanString(iterator);
            }
            else if (_nonNodeName.IsMatch(c.ToString()))
            {
                iterator.MoveBack();
                return ret;
            }

            ret += c;
        }
        return ret;
    }

    private static string ScanUntilOrEOL(CharIterator iterator, char? delim)
    {
        string ret = "";
        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();
            if (c == delim)
            {
                return ret;
            }
            ret += c;
        }
        return ret;
    }

    private static string ScanGlobalCommand(CharIterator iterator)
    {
        string ret = "";
        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();
            if (_nonAlphanum.IsMatch(c.ToString()))
            {
                iterator.MoveBack();
                return ret;
            }
            ret += c;
        }
        return ret;
    }

    public static string Sanitize(string input)
    {
        CharIterator iterator = new(input);
        string ret = "";

        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();

            if (c == '$')
            {
                string nodePath = ScanNodePath(iterator);
                ret += $"from.call(\"{nodePath}\")";
            }
            else if (c == '"')
            {
                string str = ScanString(iterator);
                ret += $"\"{str}\"";
            }
            else if (c == '\\')
            {
                // \global -> global.call
                string command = ScanGlobalCommand(iterator);
                ret += $"{command}.call";
            }
            else if (c == '=')
            {
                if (iterator.GetNext(-2) != '!')
                {
                    var val = ScanUntilOrEOL(iterator, null);
                    ret = ret.Replace("from.call", "to_node_path.call");
                    ret = $"set_prop.call({ret}, {val})";
                }
            }
            else
            {
                ret += c;
            }
        }

        return ret;
    }
}
