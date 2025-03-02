using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Core.Mediator;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediatorHandler(this IServiceCollection services, Assembly assembly)
    {
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(assembly); });

        services.AddScoped<IMediatorHandler, MediatorHandler>();

        return services;
    }
}