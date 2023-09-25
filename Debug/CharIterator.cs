namespace SupaLidlGame.Debug;

internal sealed class CharIterator : Iterator<char>
{
    public CharIterator(string str) : base(str.ToCharArray())
    {

    }

    public CharIterator(char[] chars) : base(chars)
    {

    }

    public override char MoveNext()
    {
        char c = base.MoveNext();
        if (c == '\n')
        {
            Line++;
            Column = 0;
        }
        else
        {
            Column++;
        }
        return c;
    }
}
