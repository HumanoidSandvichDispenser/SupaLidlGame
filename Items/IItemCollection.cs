namespace SupaLidlGame.Items;

public interface IItemCollection
{
    public System.Collections.Generic.IEnumerable<ItemMetadata> GetItems();

    public int Capacity { get; }
}

public interface IItemCollection<T> : IItemCollection
{
    public bool Add(T item);

    public bool Remove(T item);
}
