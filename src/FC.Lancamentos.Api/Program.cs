using FC.Lancamentos.Api.Apis;
using FC.Lancamentos.Api.Config;
using FC.ServiceDefaults;
using FC.ServiceDefaults.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
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