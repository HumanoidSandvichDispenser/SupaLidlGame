namespace SupaLidlGame;

public class InterpreterException : System.Exception
{
    public int Line { get; set; }
    public int Column { get; set; }

    public InterpreterException(string msg, int line, int column) : base(msg)
    {
        Line = line;
        Column = column;
    }
}
