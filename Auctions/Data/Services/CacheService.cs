using Microsoft.Extensions.Caching.Memory;

public class CacheService : ICacheService
{
    private IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T Retrieve<T>(string cacheKey)
    {
        _ = _cache.TryGetValue(cacheKey, out T value);
        return value;
    }

    public void Store<T>(string cacheKey, T value, TimeSpan timeSpan)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(timeSpan);

        _cache.Set(cacheKey, value, cacheEntryOptions);
    }
}
