using SupaLidlGame.Debug.Transpiler;
using Xunit.Abstractions;

namespace SupaLidlGame.UnitTests;

public class GodotTranspilerTest
{   
    private readonly ITestOutputHelper output;

    public GodotTranspilerTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Theory]
    [InlineData("abc", TokenType.Identifier)]
    [InlineData("123", TokenType.Number)]
    [InlineData("$Node/Path:Property", TokenType.NodePath)]
    public void Tokenize(string str, TokenType expectedType)
    {
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(str);
        Token[] tokens = tokenizer.Lex(iterator).ToArray();
        Assert.Equal(1, tokens.Length);
        Assert.Equal(expectedType, tokens[0].Type);
    }

    [Theory]
    [InlineData("abc", "abc")]
    [InlineData("ABC_DEF", "ABC_DEF")]
    [InlineData("\"str\"", "str")]
    public void TokenizeIdentifier(string str, string expectedValue)
    {
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(str);
        Token[] tokens = tokenizer.Lex(iterator).ToArray();
        Assert.Equal(1, tokens.Length);
        Assert.Equal(expectedValue, tokens[0].Value);
    }

    [Fact]
    public void TokenizeSource()
    {
        string source = "abc $NodePath + operator=";
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(source);
        Token[] tokens = tokenizer.Lex(iterator).ToArray();
        TokenType[] expectedTypes =
        {
            TokenType.Identifier,
            TokenType.NodePath,
            TokenType.Operator,
            TokenType.Identifier,
            TokenType.Operator,
        };
        Assert.Equal(tokens.Length, expectedTypes.Length);
        for (int i = 0; i < tokens.Length; i++)
        {
            Assert.Equal(expectedTypes[i], tokens[i].Type);
        }
    }

    [Theory]
    [InlineData("$Some/Nested", "Some/Nested")]
    [InlineData("$'Some/Nested'", "Some/Nested")]
    [InlineData("$'With A/Path and:Property'", "With A/Path and:Property")]
    [InlineData("$'broken as'hell", "broken as", 2)]
    public void TestNodePath(string source, string expectedValue, int count = 1)
    {
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(source);
        Token[] tokens = tokenizer.Lex(iterator).ToArray();
        Assert.Equal(count, tokens.Length);
        Assert.Equal(expectedValue, tokens[0].Value);
    }

    [Theory]
    [InlineData("\"this is a string\"", "this is a string")]
    [InlineData("\"escape\\\\\"", "escape\\")]
    public void TestStrings(string source, string expectedValue)
    {
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(source);
        Token[] tokens = tokenizer.Lex(iterator).ToArray();
        Assert.Equal(expectedValue, tokens[0].Value);
    }

    [Theory]
    [InlineData("()", 2)]
    [InlineData("\"escape\\\\\"", 1)]
    public void TestCount(string source, int expectedValue)
    {
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(source);
        Token[] tokens = tokenizer.Lex(iterator).ToArray();
        Assert.Equal(expectedValue, tokens.Length);
    }

    [Fact]
    public void TestAssignmentParsing()
    {
        Token[] tokens =
        {
            new(TokenType.Identifier, "x", 0, 0),
            new(TokenType.Operator, "=", 0, 0),
            new(TokenType.Identifier, "val", 0, 0),
        };
        Expression[] expr = Parser.Parse(tokens).ToArray();
        Assert.Equal(1, expr.Length);
        Assert.IsType<AssignmentExpression>(expr[0]);
    }

    [Fact]
    public void TestAssignmentGrouping()
    {
        Token[] tokens =
        {
            new(TokenType.Identifier, "x", 0, 0),
            new(TokenType.Grouping, "(", 0, 0),
            new(TokenType.Identifier, "val", 0, 0),
            new(TokenType.Operator, ",", 0, 0),
            new(TokenType.Identifier, "val", 0, 0),
            new(TokenType.Grouping, ")", 0, 0),
            new(TokenType.Operator, "+", 0, 0),
            new(TokenType.Grouping, "(", 0, 0),
            new(TokenType.Number, "2", 0, 0),
            new(TokenType.Grouping, ")", 0, 0),
        };
        Expression[] expr = Parser.Parse(tokens).ToArray();
        Assert.Equal(1, expr.Length);
        Assert.IsType<OperationExpression>(expr[0]);
        var op = expr[0] as OperationExpression;
        Assert.NotNull(op);
        var call = op.Left as CallExpression;
        Assert.NotNull(call);
        Assert.Equal(2, call.Arguments.Length);
        Assert.IsType<GroupedExpression>(op.Right);
    }

    [Fact]
    public void TestExpressionTypes()
    {
        string source = "print(\"test\")";
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(source);
        var tokens = tokenizer.Lex(iterator).ToArray();
        var exprs = Parser.Parse(tokens).ToArray();
        Assert.IsType<CallExpression>(exprs[0]);
        var call = exprs[0] as CallExpression;
        Assert.IsType<LiteralExpression>(call.Arguments[0]);
    }

    [Fact]
    public void TestExpressionTypes2()
    {
        string source = "x = \"val\"";
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(source);
        var tokens = tokenizer.Lex(iterator).ToArray();
        var exprs = Parser.Parse(tokens).ToArray();
        Assert.IsType<AssignmentExpression>(exprs[0]);
        var call = exprs[0] as AssignmentExpression;
        Assert.NotEqual(null, call.Left);
        Assert.IsType<LiteralExpression>(call.Expression);
        Assert.Equal("x", call.Left.Literal.Value);
        Assert.Equal("x", call.Left.Transpile());
        Assert.Equal("\"val\"", call.Expression.Transpile());
    }

    [Fact]
    public void TestIndividualTranspilation()
    {
        var left = new LiteralExpression(new Token(TokenType.Identifier, "x", 0, 0), 0, 0);
        var right = new LiteralExpression(new Token(TokenType.String, "lol", 0, 0), 0, 0);
        var exp = new AssignmentExpression(left, right, 0, 0);
        Assert.Equal("set(\"x\", \"lol\")", exp.Transpile());
    }

    [Theory]
    [InlineData("x = 2", "set(\"x\", 2)")]
    [InlineData("call(arg1, arg2)", "call(arg1, arg2)")]
    [InlineData("call(arg1,arg2)", "call(arg1, arg2)")]
    [InlineData("x = (a + b) *c+d + f(\"str\")", "set(\"x\", (a + b) * c + d + f(\"str\"))")]
    [InlineData("3 + My_Func(x * 2, x*5)", "3 + My_Func(x * 2, x * 5)")]
    public void TestTranspilation(string source, string transpiled)
    {
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(source);
        var tokens = tokenizer.Lex(iterator).ToArray();
        var exprs = Parser.Parse(tokens).ToArray();
        Assert.Equal(1, exprs.Length);
        Assert.Equal(transpiled, exprs[0].Transpile());
    }
}
