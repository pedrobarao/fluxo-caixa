using FC.Consolidado.Api.Apis;
using FC.Consolidado.Api.Config;
using FC.ServiceDefaults;
using FC.ServiceDefaults.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApiConfig(withApiVersioning);
builder.AddHealthCheckConfig();

var app = builder.Build();

app.MapDefaultEndpoints();

var apiConsolidado = app.NewVersionedApi("API de Consolidado");
apiConsolidado.MapConsolicadoApiV1();

app.UseDefaultOpenApiConfig();

app.UseHttpsRedirection();

app.Run();