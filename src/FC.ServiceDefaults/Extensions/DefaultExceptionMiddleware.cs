using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FC.ServiceDefaults.Extensions;

public class DefaultExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public DefaultExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var invalidOperation = TypedResults.Extensions.InvalidOperation(errors, context);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(invalidOperation.ProblemDetails);
        }
    }
}