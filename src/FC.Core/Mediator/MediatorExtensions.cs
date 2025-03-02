using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Core.Mediator;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediatorHandler(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(assembly); });

        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}