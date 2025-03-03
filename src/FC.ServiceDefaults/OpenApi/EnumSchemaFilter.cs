using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FC.ServiceDefaults.OpenApi;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            schema.Type = "string";
            schema.Format = null;

            var enumValues = Enum.GetNames(context.Type);
            schema.Enum = enumValues.Select(name => new OpenApiString(name)).Cast<IOpenApiAny>().ToList();
        }
    }
}