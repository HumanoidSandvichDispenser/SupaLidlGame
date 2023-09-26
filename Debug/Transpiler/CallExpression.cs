using System.Linq;

namespace SupaLidlGame.Debug.Transpiler;

public class CallExpression : Expression
{
    public Expression Identifier { get; set; }

    public Expression[] Arguments { get; set; }

    public CallExpression(LiteralExpression identifier, Expression[] args,
        int line, int col) : base(line, col)
    {
        Identifier = identifier;
        Arguments = args;
    }

    public override string Transpile()
    {
        var args = Arguments
            .Select((ex) => ex.Transpile())
            .Aggregate((a, b) => a + ", " + b);
        return $"{Identifier.Transpile()}({args})";
    }
}
