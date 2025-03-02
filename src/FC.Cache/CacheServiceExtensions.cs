using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FC.Cache;

public static class CacheServiceExtensions
{
    public static IServiceCollection AddRedisCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisOptions = new RedisCacheOptions();
        configuration.GetSection("Redis").Bind(redisOptions);

        services.Configure<RedisCacheOptions>(configuration.GetSection("Redis"));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisOptions.ConnectionString ?? "localhost"));

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}