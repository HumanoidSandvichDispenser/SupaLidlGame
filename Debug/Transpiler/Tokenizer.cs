using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SupaLidlGame.Debug.Transpiler;

public sealed class Tokenizer
{
    public readonly char DECIMAL_POINT = '.';
    public readonly char DECIMAL_SUBSEPARATOR = ',';
    public readonly char NODE_PATH_PREFIX = '$';

    private readonly HashSet<char> WHITESPACE = new HashSet<char>
    {
        ' ',
        '\n'
    };

    private readonly HashSet<char> OPERATOR = new HashSet<char>
    {
        '+',
        '-',
        '*',
        '/',
        '.',
        ',',
        '=',
        '!',
    };

    private readonly HashSet<char> GROUPING = new HashSet<char>
    {
        '(',
        ')',
    };

    private readonly HashSet<char> STRING_DELIM = new HashSet<char>
    {
        '"',
        '\'',
    };

    private readonly Regex REGEX_NUMBER = new Regex("[.0-9]");
    private readonly Regex REGEX_IDENTIFIER_START = new Regex("[_a-zA-Z]");
    private readonly Regex REGEX_IDENTIFIER = new Regex("[_a-zA-Z0-9]");
    private Regex NON_NODE_PATH = new("[^a-zA-Z0-9_\\-\\/\\.\\:]");

    private static string ScanString(CharIterator iterator, char delim = '"')
    {
        string ret = "";

        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();

            if (c == delim)
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
                        throw new InterpreterException("Unexpected EOL, " +
                            "expected proper string termination",
                            iterator.Line, iterator.Column);
                    default:
                        ret += escape;
                        break;
                }
            }
            else
            {
                ret += c;
            }
        }
        throw new InterpreterException($"Unexpected EOL, expected: {delim}",
            iterator.Line, iterator.Column);
    }

    private string ScanNodePath(CharIterator iterator)
    {
        string ret = "";
        bool isAtStart = true;
        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();

            if (isAtStart && STRING_DELIM.Contains(c))
            {
                isAtStart = false;
                return ScanString(iterator, c);
            }
            else if (NON_NODE_PATH.IsMatch(c.ToString()))
            {
                iterator.MoveBack();
                return ret;
            }

            isAtStart = false;
            ret += c;
        }
        return ret;
    }

    private string ScanRegex(CharIterator iterator, Regex regex)
    {
        string ret = "";
        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();

            if (!regex.IsMatch(c.ToString()))
            {
                iterator.MoveBack();
                return ret;
            }

            ret += c;
        }
        return ret;
    }

    public IEnumerable<Token> Lex(CharIterator iterator)
    {
        //Token curToken = new Token(TokenType.Any, );
        while (iterator.GetNext() != default)
        {
            char c = iterator.MoveNext();
            int line = iterator.Line;
            int col = iterator.Column;

            if (GROUPING.Contains(c))
            {
                yield return new Token(TokenType.Grouping,
                    c.ToString(), line, col);
            }
            else if (OPERATOR.Contains(c))
            {
                yield return new Token(TokenType.Operator,
                    c.ToString(), line, col);
            }
            else if (c == NODE_PATH_PREFIX)
            {
                yield return new Token(TokenType.NodePath,
                    ScanNodePath(iterator), line, col);
            }
            else if (STRING_DELIM.Contains(c))
            {
                yield return new Token(TokenType.String,
                    ScanString(iterator, c), line, col);
            }
            else if (REGEX_IDENTIFIER_START.IsMatch(c.ToString()))
            {
                yield return new Token(TokenType.Identifier,
                    c + ScanRegex(iterator, REGEX_IDENTIFIER), line, col);
            }
            else if (REGEX_NUMBER.IsMatch(c.ToString()))
            {
                yield return new Token(TokenType.Number,
                    c + ScanRegex(iterator, REGEX_NUMBER), line, col);
            }
            else if (WHITESPACE.Contains(c))
            {
                continue;
            }
            else
            {
                throw new InterpreterException($"Unknown symbol {c}",
                    line, col);
            }
        }
    }
}
