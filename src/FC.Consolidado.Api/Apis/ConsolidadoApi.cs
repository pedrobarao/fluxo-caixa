using FC.Consolidado.Application.Outputs;
using FC.Consolidado.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FC.Consolidado.Api.Apis;

public static class ConsolidadoApi
{
    public static RouteGroupBuilder MapConsolicadoApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/relatorio/saldo-consolidado").HasApiVersion(1.0);

        api.MapGet("/", SaldoConsolidado);

        return api;
    }

    private static async IAsyncEnumerable<SaldoConsolidadoOutput> SaldoConsolidado(DateOnly data,
        [FromServices] ISaldoConsolidadoQuery query)
    {
        await foreach (var saldo in query.ObterPorData(data)) yield return saldo;
    }
}