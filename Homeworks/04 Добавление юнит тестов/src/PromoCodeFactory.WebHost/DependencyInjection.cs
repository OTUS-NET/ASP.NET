namespace PromoCodeFactory.WebHost;

public static class DependencyInjection
{
    private const string _apiName = "PromoCodeFactory API";
    private const string _apiVersion = "v1";

    public static void AddOpenApi(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddOpenApi(_apiVersion, options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = $"{_apiName} ({env.EnvironmentName})";
                document.Info.Version = _apiVersion;
                return Task.CompletedTask;
            });
        });
    }

    public static void MapSwaggerUI(this IApplicationBuilder app)
    {
        app.UseSwaggerUI(op =>
            op.SwaggerEndpoint($"/openapi/{_apiVersion}.json", _apiName));
    }
}
