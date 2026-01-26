namespace PromoCodeFactory.WebHost;

public static class DependencyInjection
{
    public static void AddOpenApi(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = $"PromoCodeFactory API ({env.EnvironmentName})";
                document.Info.Version = "v1";
                return Task.CompletedTask;
            });
        });
    }
}
