namespace SupaLidlGame.Debug.Transpiler;

public class OperationExpression : Expression
{
    public Expression Left { get; set; }

    public Expression Right { get; set; }

    public Token Operator { get; set; }

    public OperationExpression(Expression left, Token token, Expression right,
        int line, int col) : base(line, col)
    {
        if (token.Type != TokenType.Operator)
        {
            throw new InterpreterException(
                $"Expected operator, got {token.Value}",
                token.Line,
                token.Column);
        }
        Left = left;
        Operator = token;
        Right = right;
    }

    public override string Transpile()
    {
        var left = Left.Transpile();
        var right = Right.Transpile();
        var op = Operator.Value;
        if (op == ".")
        {
            return $"{left}{op}{right}";
        }
        else
        {
            return $"{left} {op} {right}";
        }
    }
}
