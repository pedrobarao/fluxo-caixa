using Asp.Versioning;
using FC.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FC.ServiceDefaults.OpenApi;

public static class DefaultOpenApiConfig
{
    public static IHostApplicationBuilder AddDefaultOpenApiConfig(
        this IHostApplicationBuilder builder,
        IApiVersioningBuilder? apiVersioning = null)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var openApi = configuration.GetSection("OpenApi");

        if (!openApi.Exists()) return builder;

        services.AddEndpointsApiExplorer();

        if (apiVersioning is not null)
        {
            // the default format will just be ApiVersion.ToString(); for example, 1.0.
            // this will format the version as "'v'major[.minor][-status]"
            apiVersioning.AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerDefaultOptions>();

            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<OpenApiDefaultValues>();

                // Configuração para usar strings em enums no Swagger
                options.UseInlineDefinitionsForEnums();
                options.UseAllOfToExtendReferenceSchemas();
                options.SchemaFilter<EnumSchemaFilter>();
            });
        }

        return builder;
    }

    public static IApplicationBuilder UseDefaultOpenApiConfig(this WebApplication app)
    {
        var configuration = app.Configuration;
        var openApiSection = configuration.GetSection("OpenApi");

        if (!openApiSection.Exists()) return app;

        app.UseSwagger();

        // if (app.Environment.IsDevelopment())
        // {
        app.UseSwaggerUI(setup =>
        {
            var pathBase = configuration["PATH_BASE"] ?? string.Empty;
            var authSection = openApiSection.GetSection("Auth");
            var endpointSection = openApiSection.GetRequiredSection("Endpoint");

            foreach (var description in app.DescribeApiVersions())
            {
                var name = description.GroupName;
                var url = endpointSection["Url"] ?? $"{pathBase}/swagger/{name}/swagger.json";

                setup.SwaggerEndpoint(url, name);
            }

            if (authSection.Exists())
            {
                setup.OAuthClientId(authSection.GetRequiredValue("ClientId"));
                setup.OAuthAppName(authSection.GetRequiredValue("AppName"));
            }
        });

        // Add a redirect from the root of the app to the swagger endpoint
        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
        // }

        return app;
    }
}