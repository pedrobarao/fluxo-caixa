using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace FC.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly RedisCacheOptions _options;
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisCacheOptions> options)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _database = _redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(GetFullKey(key));

        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
    {
        var expiry = expirationTime ?? _options.DefaultExpirationTime;
        var serializedValue = JsonSerializer.Serialize(value);

        return await _database.StringSetAsync(
            GetFullKey(key),
            serializedValue,
            expiry);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _database.KeyDeleteAsync(GetFullKey(key));
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(GetFullKey(key));
    }

    private string GetFullKey(string key)
    {
        return string.IsNullOrEmpty(_options.InstanceName)
            ? key
            : $"{_options.InstanceName}:{key}";
    }
}