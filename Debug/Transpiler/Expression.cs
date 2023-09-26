namespace SupaLidlGame.Debug.Transpiler;

public abstract class Expression
{
    public int Line { get; set; }

    public int Column { get; set; }

    public Expression(int line, int col)
    {
        Line = line;
        Column = col;
    }

    public abstract string Transpile();
}
