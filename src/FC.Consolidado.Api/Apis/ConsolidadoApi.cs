using FC.Consolidado.Application.DTOs;
using FC.Consolidado.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FC.Consolidado.Api.Apis;

public static class ConsolidadoApi
{
    public static RouteGroupBuilder MapConsolicadoApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/consolidado").HasApiVersion(1.0);

        api.MapGet("/", SaldoConsolidado);

        return api;
    }

    private static async Task<SaldoConsolidadoDto> SaldoConsolidado(DateOnly data,
        [FromServices] ISaldoConsolidadoQuery query)
    {
        return await query.ObterPorData(data);
    }
}