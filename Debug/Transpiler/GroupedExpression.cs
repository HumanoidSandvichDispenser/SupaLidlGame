namespace SupaLidlGame.Debug.Transpiler;

public class GroupedExpression : Expression
{
    public Expression Root { get; set; }

    public GroupedExpression(Expression root, int line, int col)
        : base(line, col)
    {
        Root = root;
    }

    public override string Transpile()
    {
        return $"({Root.Transpile()})";
    }
}
