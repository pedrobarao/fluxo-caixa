using FC.Core.Mediator;
using FC.Lancamentos.Api.Apis;
using FC.Lancamentos.Api.Config;
using FC.ServiceDefaults;
using FC.ServiceDefaults.OpenApi;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddScoped<IMediatorHandler, MediatorHandler>();

builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApiConfig(withApiVersioning);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

app.MapDefaultEndpoints();

var apiLancamentos = app.NewVersionedApi("API de Lançamentos");
apiLancamentos.MapLancamentosApiV1();

app.UseDefaultOpenApiConfig();

app.UseHttpsRedirection();

app.Run();