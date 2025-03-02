namespace FC.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);

    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expirationTime = null);

    Task<bool> RemoveAsync(string key);

    Task<bool> ExistsAsync(string key);
}