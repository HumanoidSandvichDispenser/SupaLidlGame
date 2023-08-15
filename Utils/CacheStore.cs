using Godot;
using System.Collections.Generic;

public class CacheStore<TKey, TVal>
{
    // default TTL is 1 min
    public ulong TimeToLive { get; } = 60000;

    private Dictionary<TKey, CacheItem<TVal>> _store = new();

    public CacheItem<TVal> this[TKey key]
    {
        get
        {
            if (_store.ContainsKey(key))
            {
                return _store[key];
            }
            return null;
        }
        set
        {
            if (_store.ContainsKey(key))
            {
                _store[key] = value;
            }
            else
            {
                _store.Add(key, value);
            }
        }
    }

    public TVal Retrieve(TKey key)
    {
        if (IsItemValid(key))
        {
            return _store[key].Value;
        }
        return default;
    }

    public void Update(TKey key, TVal value = default)
    {
        CacheItem<TVal> cacheItem;
        if (_store.ContainsKey(key))
        {
            cacheItem = _store[key];
        }
        else
        {
            cacheItem = new CacheItem<TVal>();
            _store[key] = cacheItem;
        }
        cacheItem.TimeToLiveTimestamp = Time.GetTicksMsec();
        cacheItem.Value = value;
    }

    public bool IsItemStale(TKey key)
    {
        if (_store.ContainsKey(key))
        {
            return _store[key].HasExpired(TimeToLive);
        }
        return false;
    }

    public bool IsItemValid(TKey key)
    {
        return key is not null && _store.ContainsKey(key) && !IsItemStale(key);
    }

    public bool ContainsKey(TKey key)
    {
        return _store.ContainsKey(key);
    }

    public void Update(TKey key)
    {
        if (_store.ContainsKey(key))
        {
            _store[key].TimeToLiveTimestamp = Time.GetTicksMsec();
        }
        else
        {
            GD.PushWarning("Updating a non-existent item in a cache!");
        }
    }
}
