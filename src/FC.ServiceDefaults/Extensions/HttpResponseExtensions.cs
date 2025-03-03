using FC.Core.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FC.ServiceDefaults.Extensions;

public static class HttpResponseExtensions
{
    public static ValidationProblem InvalidOperation(this IResultExtensions _,
        List<Error> errors,
        HttpContext context)
    {
        var errorsDictionary =
            errors.ToDictionary<Error?, string, string[]>(_ => "details", error => [error.ToString()]);

        return InvalidOperation(_, errorsDictionary, context);
    }

    public static ValidationProblem InvalidOperation(this IResultExtensions _,
        IDictionary<string, string[]> errors,
        HttpContext context)
    {
        return TypedResults.ValidationProblem(type: "https://datatracker.ietf.org/doc/html/rfc7807",
            title: "Erro ao processar a requisição.",
            instance: context.Request.Path,
            detail: "Um ou mais erros ocorreram ao processar a requisição.",
            errors: errors);
    }
}