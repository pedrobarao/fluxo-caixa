namespace FC.Cache;

public class CachedRepository<TEntity, TKey> where TEntity : class
{
    private readonly TimeSpan _cacheExpiration;
    private readonly string _cachePrefix;
    private readonly ICacheService _cacheService;

    public CachedRepository(
        ICacheService cacheService,
        string cachePrefix,
        TimeSpan? cacheExpiration = null)
    {
        _cacheService = cacheService;
        _cachePrefix = cachePrefix;
        _cacheExpiration = cacheExpiration ?? TimeSpan.FromMinutes(30);
    }

    public async Task<TEntity?> GetAsync(TKey key, Func<TKey, Task<TEntity?>> getFromSource)
    {
        var cacheKey = $"{_cachePrefix}:{key}";

        var cachedItem = await _cacheService.GetAsync<TEntity>(cacheKey);
        if (cachedItem != null)
            return cachedItem;

        var item = await getFromSource(key);
        if (item != null)
            await _cacheService.SetAsync(cacheKey, item, _cacheExpiration);

        return item;
    }

    public async Task InvalidateCacheAsync(TKey key)
    {
        var cacheKey = $"{_cachePrefix}:{key}";
        await _cacheService.RemoveAsync(cacheKey);
    }
}