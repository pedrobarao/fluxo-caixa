using FC.Core.Mediator;
using FC.Lancamentos.Api.Application.Commands;
using FC.Lancamentos.Api.Domain.Events;
using FC.MessageBus;
using MassTransit;
using RabbitMQ.Client;

namespace FC.Lancamentos.Api.Config;

public static class DependencyInjectionConfig
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterMediatorHandler(services);
        RegisterMessageBus(services, configuration);

        return services;
    }

    private static void RegisterMediatorHandler(IServiceCollection services)
    {
        services.AddMediatorHandler(typeof(NovaTransacaoCommand).Assembly);
    }

    private static void RegisterMessageBus(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBus(configuration, customConfig: (context, cfg) =>
        {
            cfg.Message<TransacaoCriadaEvent>(x => x.SetEntityName("lancamentos-exchange"));
            cfg.Publish<TransacaoCriadaEvent>(x => x.ExchangeType = ExchangeType.Topic);
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
}