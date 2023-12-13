using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Filters;

public class HangfireDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.ExternalDocs = new OpenApiExternalDocs
        {
            Description = "Hangfire Dashboard",
            Url = new Uri("/hangfire", UriKind.Relative),
        };
    }
}
