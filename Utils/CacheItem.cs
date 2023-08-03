using Godot;

public class CacheItem<T>
{
    public ulong TimeToLiveTimestamp { get; set; }

    public T Value { get; set; }

    public bool HasExpired(ulong ttl)
    {
        return Time.GetTicksMsec() > TimeToLiveTimestamp + ttl;
    }
}
