using System.Collections.Generic;

namespace SupaLidlGame.Debug;

internal sealed class Tokenizer
{
    private static readonly HashSet<char> WHITESPACE = new HashSet<char> { ' ', '\n' };

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
                            iterator.Line, iterator.Column);
                    default:
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
            char c = iterator.GetNext();

            if (c == '"')
            {
                ret += ScanString(iterator);
            }
            else if (WHITESPACE.Contains(c))
            {
                return ret;
            }

            ret += c;
        }
        return ret;
    }

    /*
    private static string ScanUntil(CharIterator iterator)
    {

    }
    */

    private static string ScanExpression(CharIterator iterator)
    {
        int level = 0;
        string exp = "";
        while (iterator.GetNext() != '\0')
        {
            char c = iterator.GetNext();

            if (c == '(')
            {
                level++;
            }
            else if (c == ')')
            {
                level--;
            }

            if (level < 0)
            {
                return exp;
            }

            exp += c;
        }
        return exp;
    }

    private static string ScanUntilOrEOL(CharIterator iterator, char delim)
    {
        string ret = "";
        while (iterator.GetNext() != '\0')
        {
            char c = iterator.GetNext();
            if (c == delim)
            {
                return ret;
            }
            ret += c;
        }
        return ret;
    }

    public static IEnumerable<Token> Tokenize(CharIterator iterator)
    {
        System.Diagnostics.Debug.Print("hi");
        while (iterator.GetNext() != '\0')
        {
            char curChar = iterator.MoveNext();
            System.Diagnostics.Debug.Print(curChar.ToString());

            int line = iterator.Line;
            int col = iterator.Column;

            if (WHITESPACE.Contains(curChar))
            {
                continue;
            }
            else if (curChar == '\\')
            {
                string command = ScanUntilOrEOL(iterator, ' ');
                if (command == "")
                {
                    throw new InterpreterException(
                        "Expected a command name",
                        iterator.Line,
                        iterator.Column);
                }
                yield return new Token(TokenType.Command,
                    command,
                    line,
                    col);
            }
            else if (curChar == '(')
            {
                string exp = ScanExpression(iterator);
                yield return new Token(TokenType.GodotExpression,
                    exp,
                    line,
                    col);
            }
            else if (curChar == '"')
            {
                yield return new Token(TokenType.String,
                    ScanString(iterator),
                    line,
                    col);
            }
            else
            {
                // parse this as expression
                string exp = ScanUntilOrEOL(iterator, ' ');
                yield return new Token(TokenType.GodotExpression,
                    exp,
                    line,
                    col);
            }
            /*
            else if (curChar == '$')
            {
                yield return new Token(TokenType.NodePath,
                    ScanNodePath(iterator),
                    line,
                    col);
            }
            */
        }
        yield return new Token(TokenType.End, "",
            iterator.Line, iterator.Column);
    }
}

