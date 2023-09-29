using System.Text.RegularExpressions;

namespace SupaLidlGame.Debug.Transpiler;

public class LiteralExpression : Expression
{
    public Token Literal { get; set; }

    public LiteralExpression(Token literal, int line, int col)
        : base(line, col)
    {
        Literal = literal;
    }

    public string EscapedLiteral()
    {
        return Literal.Value.Replace("\"", "\\\"")
            .Replace("'", "\\'")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");
    }

    public override string Transpile()
    {
        var val = EscapedLiteral();
        if (Literal.Type == TokenType.NodePath)
        {
            return $"from.call(\"{val}\")";
        }
        else if (Literal.Type == TokenType.String)
        {
            return $"\"{val}\"";
        }
        return Literal.Value;
    }

    public string TranspileNodePath()
    {
        var val = EscapedLiteral();
        return $"to_node_path.call(\"{val}\")";
    }
}
