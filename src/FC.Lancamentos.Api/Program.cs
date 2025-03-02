using System.Text.Json;
using System.Text.Json.Serialization;
using FC.Cache;
using FC.Core.Mediator;
using FC.Lancamentos.Api.Apis;
using FC.Lancamentos.Api.Config;
using FC.ServiceDefaults;
using FC.ServiceDefaults.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddScoped<IMediatorHandler, MediatorHandler>();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApiConfig(withApiVersioning);

var app = builder.Build();

app.MapDefaultEndpoints();

var apiLancamentos = app.NewVersionedApi("API de Lançamentos");
apiLancamentos.MapLancamentosApiV1();

app.UseDefaultOpenApiConfig();

app.UseHttpsRedirection();

app.Run();