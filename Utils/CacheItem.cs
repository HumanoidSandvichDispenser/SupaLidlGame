using Godot;

public class CacheItem<T>
{
    public ulong TimeToLiveTimestamp { get; set; }

    public T Value { get; set; }

    public bool HasExpired(ulong ttl)
    {
        // this specific value indicates the item is manually staled
        if (TimeToLiveTimestamp == ulong.MaxValue)
        {
            return true;
        }
        return Time.GetTicksMsec() > TimeToLiveTimestamp + ttl;
    }

    public void Stale()
    {
        TimeToLiveTimestamp = ulong.MaxValue;
    }
}
