namespace SupaLidlGame.Debug.Transpiler;

public class AssignmentExpression : Expression
{
    public LiteralExpression Left { get; set; }

    public Expression Expression { get; set; }

    public AssignmentExpression(LiteralExpression left, Expression expression,
        int line, int col) : base(line, col)
    {
        Left = left;
        Expression = expression;
    }

    public override string Transpile()
    {
        var right = Expression.Transpile();
        if (Left.Literal.Type == TokenType.NodePath)
        {
            return $"set_prop.call({Left.TranspileNodePath()}, {right})";
        }
        return $"set(\"{Left.Transpile()}\", {right})";
    }
}
