using Microsoft.AspNetCore.Http.HttpResults;

namespace FC.Consolidado.Api.Apis;

public static class ConsolidadoApi
{
    public static RouteGroupBuilder MapConsolicadoApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/relatorio/saldo-consolidado").HasApiVersion(1.0);

        api.MapPost("/", SaldoConsolidado);

        return api;
    }

    private static async Task<Results<Created, ValidationProblem>> SaldoConsolidado(HttpContext context, DateOnly data)
    {
        return TypedResults.Created();
    }
}