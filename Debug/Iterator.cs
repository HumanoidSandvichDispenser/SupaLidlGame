using System.Collections.Generic;

namespace SupaLidlGame.Debug;

public class Iterator<T> where T : struct
{
    public int Index { get; protected set; } = -1;

    protected List<T> _elements;

    public Iterator(T[] elements)
    {
        _elements = new List<T>(elements);
    }

    public Iterator(List<T> elements)
    {
        _elements = new List<T>(elements);
    }

    public T GetNext(int offset = 0)
    {
        if (Index + offset + 1 < _elements.Count)
        {
            return _elements[Index + offset + 1];
        }

        return default;
    }

    public virtual T MoveNext()
    {
        T next = GetNext();
        Index++;
        return next;
    }

    public virtual void MoveBack()
    {
        Index--;
    }
}
