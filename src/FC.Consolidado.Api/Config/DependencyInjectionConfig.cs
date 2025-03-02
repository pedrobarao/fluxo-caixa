using FC.Consolidado.Infra.Data;
using FC.MessageBus;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace FC.Consolidado.Api.Config;

public static class DependencyInjectionConfig
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ConsolidadoDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
        });

        services.AddMessageBus(configuration, (context, cfg) =>
        {
            cfg.ReceiveEndpoint("consolidado-queue", e =>
            {
                e.Bind("lancamentos-exchange", x =>
                {
                    x.RoutingKey = "#";
                    x.ExchangeType = ExchangeType.Topic;
                });

                e.ConfigureConsumers(context);
            });

            cfg.UseMessageRetry(r => { r.Interval(3, TimeSpan.FromMinutes(1)); });
        });

        services.AddStackExchangeRedisCache(options =>
        {
            var connectionString = configuration.GetConnectionString("Redis")
                                   ?? throw new InvalidOperationException(
                                       "A string de conexão 'Redis' não foi encontrada na configuração");

            options.Configuration = connectionString;
            options.InstanceName = "FC_Consolidado_";
        });

        return services;
    }
}