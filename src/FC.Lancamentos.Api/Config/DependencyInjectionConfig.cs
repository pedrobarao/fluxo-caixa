﻿using FC.Core.Mediator;
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
        services.AddMediatorHandler(typeof(NovaTransacaoCommand).Assembly);
        services.AddMessageBus(configuration, (context, cfg) =>
        {
            cfg.Message<LancamentoRealizadoEvent>(x => x.SetEntityName("lancamentos-exchange"));
            cfg.Publish<LancamentoRealizadoEvent>(x => x.ExchangeType = ExchangeType.Topic);
            cfg.UseMessageRetry(r => r.Immediate(3));
        });

        return services;
    }
}