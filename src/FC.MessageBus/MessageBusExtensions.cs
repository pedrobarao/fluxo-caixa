using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FC.MessageBus;

public static class MessageBusExtensions
{
    public static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRegistrationConfigurator>? configureConsumers = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? customConfig = null)
    {
        services.AddMassTransit(bus =>
        {
            configureConsumers?.Invoke(bus);

            bus.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration.GetConnectionString("RabbitMQ")
                                       ?? throw new InvalidOperationException(
                                           "A string de conexão 'RabbitMQ' não foi encontrada na configuração");

                cfg.Host(connectionString);

                customConfig?.Invoke(context, cfg);

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddSingleton<IMessageBus, MessageBus>();

        return services;
    }
}