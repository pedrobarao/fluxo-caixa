using System.Data;
using FC.Cache;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FC.Consolidado.Api.Config;

public static class HealthCheckConfig
{
    public static IHostApplicationBuilder AddHealthCheckConfig(this IHostApplicationBuilder builder)
    {
        var redisOptions = new RedisCacheOptions();
        builder.Configuration.GetRequiredSection("Redis").Bind(redisOptions);

        var postgresConnectionString = builder.Configuration.GetConnectionString("PostgreSQL") ??
                                       throw new NoNullAllowedException();

        builder.Services.AddHealthChecks()
            .AddNpgSql(
                postgresConnectionString,
                name: "PostgreSQL",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"])
            .AddRedis(
                redisOptions.ConnectionString!,
                "Redis",
                HealthStatus.Unhealthy,
                ["ready"]);


        return builder;
    }
}