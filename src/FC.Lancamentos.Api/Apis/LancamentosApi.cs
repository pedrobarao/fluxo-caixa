using FC.Core.Mediator;
using FC.Lancamentos.Api.Application.Commands;
using FC.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FC.Lancamentos.Api.Apis;

public static class LancamentosApi
{
    public static RouteGroupBuilder MapLancamentosApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/lancamentos").HasApiVersion(1.0);

        api.MapPost("/", Lancamentos);

        return api;
    }

    private static async Task<Results<Created, ValidationProblem>> Lancamentos(
        HttpContext context,
        IMediatorHandler mediator,
        [FromBody] NovaTransacaoCommand command)
    {
        var result = await mediator.Send(command);

        if (result!.IsFailure) return TypedResults.Extensions.InvalidOperation(result.Errors, context);

        return TypedResults.Created();
    }
}