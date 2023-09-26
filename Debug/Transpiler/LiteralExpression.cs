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

    public override string Transpile()
    {
        if (Literal.Type == TokenType.NodePath)
        {
            var val = Regex.Escape(Literal.Value);
            return $"from.call(\"{val}\")";
        }
        else if (Literal.Type == TokenType.String)
        {
            return $"\"{Literal.Value}\"";
        }
        return Literal.Value;
    }

    public string TranspileNodePath()
    {
        var val = Regex.Escape(Literal.Value);
        return $"to_node_path.call(\"{val}\")";
    }
}
