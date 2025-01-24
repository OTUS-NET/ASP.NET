using Microsoft.OpenApi.Models;

namespace AspApiBasic.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
       services.AddEndpointsApiExplorer();
       services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "Дэмо API",
                    Version = "v1",
                    Description = "Описание эндпоинтов"
                });
            options.CustomSchemaIds(x => x.FullName);
        });
        return services;
    }
}