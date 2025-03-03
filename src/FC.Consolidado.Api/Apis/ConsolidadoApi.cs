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

    private static async Task<SaldoConsolidadoOutput> SaldoConsolidado(DateOnly data,
        [FromServices] ISaldoConsolidadoQuery query)
    {
        return await query.ObterPorData(data);
    }
}