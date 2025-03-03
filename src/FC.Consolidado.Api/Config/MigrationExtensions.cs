using FC.Consolidado.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;

namespace FC.Consolidado.Api.Config;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        var retryPolicy = Policy.Handle<NpgsqlException>()
            .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(15)
                },
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine(
                        $"Tentativa {retryCount} falhou: {exception.Message}. Aguardando {timeSpan} antes da próxima tentativa.");
                });

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConsolidadoDbContext>();
        retryPolicy.Execute(() => dbContext.Database.Migrate());
    }
}