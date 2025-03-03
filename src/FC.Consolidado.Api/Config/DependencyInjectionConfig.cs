using FC.Cache;
using FC.Consolidado.Application.Commands;
using FC.Consolidado.Application.Queries;
using FC.Consolidado.Application.Services;
using FC.Consolidado.Domain.Repositories;
using FC.Consolidado.Infra.Data;
using FC.Consolidado.Infra.Data.Repositories;
using FC.Core.Mediator;
using FC.MessageBus;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace FC.Consolidado.Api.Config;

public static class DependencyInjectionConfig
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterMessageBus(services, configuration);
        RegisterCache(services, configuration);
        RegisterDatabase(services, configuration);
        RegisterMediatorHandler(services);
        RegisterApplicationServices(services);

        return services;
    }

    private static void RegisterMediatorHandler(IServiceCollection services)
    {
        services.AddMediatorHandler(typeof(NovaTransacaoCommand).Assembly);
    }

    private static void RegisterMessageBus(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBus(
            configuration,
            consumers => { consumers.AddConsumer<TransacaoConsumerService>(); },
            (context, cfg) =>
            {
                cfg.ReceiveEndpoint("consolidado-queue", e =>
                {
                    e.Bind("lancamentos-exchange", x =>
                    {
                        x.ExchangeType = ExchangeType.Topic;
                    });

                    e.ConfigureConsumers(context);
                });

                cfg.UseMessageRetry(r => { r.Interval(3, TimeSpan.FromSeconds(30)); });
                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });
            });
    }

    private static void RegisterCache(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRedisCache(configuration);
    }

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();
        services.AddScoped<ISaldoConsolidadoQuery, SaldoConsolidadoQuery>();
    }


    private static void RegisterDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ConsolidadoDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
        });
    }
}