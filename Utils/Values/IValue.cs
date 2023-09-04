namespace SupaLidlGame;

public interface IValue<T>
{
    public delegate void ChangedEventHandler(T oldValue, T newValue);

    public T Value { get; set; }
}
