public interface ICacheService
{
    T Retrieve<T>(string cacheKey);
    void Store<T>(string cacheKey, T value, TimeSpan timeSpan);
}
