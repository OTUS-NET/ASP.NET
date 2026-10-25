using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Pcf.Common.Web;

public static class OpenApiExtensions
{
    private const string _apiVersion = "v1";

    public static void AddOpenApi(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddOpenApi(_apiVersion, options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = $"{env.ApplicationName} ({env.EnvironmentName})";
                document.Info.Version = _apiVersion;
                return Task.CompletedTask;
            });
        });
    }

    public static void MapSwaggerUI(this IApplicationBuilder app)
    {
        var environment = app.ApplicationServices
            .GetRequiredService<IWebHostEnvironment>();

        app.UseSwaggerUI(op =>
            op.SwaggerEndpoint($"/openapi/{_apiVersion}.json", environment.ApplicationName));
    }
}
