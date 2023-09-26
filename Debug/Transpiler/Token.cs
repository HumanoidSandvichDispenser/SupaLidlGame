namespace SupaLidlGame.Debug.Transpiler;

public enum TokenType
{
    None,
    Identifier,
    Grouping,
    Operator,
    String,
    Number,
    NodePath,
}

public struct Token
{
    //public static Token EndToken => new Token(TokenType.End, ";", -1, -1);
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

#if DEBUG
    public override string ToString()
    {
        return $"{Type} - {Value}\t\t@{Line}:{Column}";
    }
#endif

    public bool CompareTypeValue(Token token)
    {
        return Type == token.Type && Value == token.Value;
    }

    public override bool Equals(object o)
    {
        return base.Equals(o);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool IsLiteral => Type == TokenType.String ||
        Type == TokenType.Number ||
        Type == TokenType.NodePath;

    public bool IsSymbol => IsLiteral || Type == TokenType.Identifier;

    public static bool operator ==(Token left, Token right)
    {
        return left.Type == right.Type && left.Value == right.Value;
    }

    public static bool operator !=(Token left, Token right) => !(left == right);
}
