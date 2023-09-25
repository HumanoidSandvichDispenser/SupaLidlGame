namespace SupaLidlGame.Debug;

public enum TokenType
{
    None,
    Identifier,
    String,
    GodotExpression,
    Command,
    End
};

public struct Token
{
    public TokenType Type { get; set; }

    public string Value { get; set; }

    public int Line { get; set; }

    public int Column { get; set; }

    public Token(TokenType type, string value, int line, int col)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = col;
    }

    public bool CompareTypeValue(Token token)
    {
        return Type == token.Type && Value == token.Value;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(Token left, Token right) => left.Equals(right);

    public static bool operator !=(Token left, Token right) => !left.Equals(right);
}
