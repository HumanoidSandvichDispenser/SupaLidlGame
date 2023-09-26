using System.Collections.Generic;
using System.Linq;

namespace SupaLidlGame.Debug.Transpiler;

public class Parser
{
    private HashSet<Token> _endTokens = null;

    private Iterator<Token> _iterator;

    private static readonly Token ARGS_DELIM = new Token(TokenType.Operator, ",", 0, 0);

    private static readonly Token CLOSE_DELIM = new Token(TokenType.Grouping, ")", 0, 0);

    private Parser(Token[] tokens)
    {
        _iterator = new(tokens);
        _endTokens = new HashSet<Token> { default };
    }

    private Parser(Iterator<Token> iterator, HashSet<Token> endTokens)
    {
        _iterator = iterator;
        _endTokens = endTokens;
    }

    public GroupedExpression GroupedExpression()
    {
        Parser p = new Parser(_iterator, new HashSet<Token> { CLOSE_DELIM });
        var next = p.NextExpression(null);
        Expect(CLOSE_DELIM);
        _iterator.MoveNext();
        return new GroupedExpression(next, next.Line, next.Column);
    }

    public IEnumerable<Expression> DelimitedExpressions(Token delim, Token end)
    {
        Expect(end);

        var endTokens = new HashSet<Token> { delim, end };

        Parser p = new Parser(_iterator, endTokens);
        var next = _iterator.GetNext();
        while (next != end)
        {
            var expr = p.NextExpression(null);
            if (expr is not null)
            {
                yield return expr;
            }
            next = _iterator.MoveNext();
        }
    }

    public Expression NextExpression(Expression prev)
    {
        foreach (var end in _endTokens)
        {
            if (end == _iterator.GetNext())
            {
                return prev;
            }
        }

        var token = _iterator.MoveNext();

        if (prev is null && token.IsSymbol)
        {
            var exp = new LiteralExpression(token, token.Line, token.Column);
            return NextExpression(exp);
        }
        else if (token.Type == TokenType.Operator)
        {
            Expression right = NextExpression(null);
            if (token.Value == "=")
            {
                if (prev is not LiteralExpression l)
                {
                    throw new InterpreterException("Invalid assignment",
                        prev.Line, prev.Column);
                }
                var assignment = new AssignmentExpression(l, right,
                    token.Line, token.Column);
                return NextExpression(assignment);
            }

            var exp = new OperationExpression(
                prev, token, right, token.Line, token.Column);
            return NextExpression(exp);
        }
        else if (token.Type == TokenType.Grouping)
        {
            if (token.Value == ")")
            {
                throw new InterpreterException("Unexpected )",
                    token.Line, token.Column);
            }
            if (prev is LiteralExpression l)
            {
                // this is a function call
                Expression[] args =
                    DelimitedExpressions(ARGS_DELIM, CLOSE_DELIM)
                    .ToArray();
                var c = new CallExpression(l, args, token.Line, token.Column);
                return NextExpression(c);
            }
            else
            {
                // otherwise it's just a grouping
                return NextExpression(GroupedExpression());
            }
        }

        throw new InterpreterException($"Unexpected token {token.Value}",
            token.Line, token.Column);
    }

    public void Expect(Token token)
    {
        var next = _iterator.GetNext();
        if (next == default)
        {
            var cur = _iterator.GetNext(-1);
            throw new InterpreterException($"Expected {token.Value}",
                cur.Line, cur.Column);
        }
    }

    public static IEnumerable<Expression> Parse(Token[] tokens)
    {
        var parser = new Parser(tokens);

        var iterator = parser._iterator;
        while (iterator.GetNext() != default)
        {
            var expr = parser.NextExpression(null);
            if (expr is not null)
            {
                yield return expr;
            }
            iterator.MoveNext();
        }
    }
}
