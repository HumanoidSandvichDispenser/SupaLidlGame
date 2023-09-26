using System.Linq;

namespace SupaLidlGame.Debug.Transpiler;

public static class Transpiler
{
    public static string Transpile(string source)
    {
        var tokenizer = new Debug.Transpiler.Tokenizer();
        SupaLidlGame.Debug.CharIterator iterator = new(source);
        var tokens = tokenizer.Lex(iterator).ToArray();
        var exprs = Parser.Parse(tokens).ToArray();
        return exprs[0].Transpile();
    }
}
