using Microsoft.AspNetCore.Mvc;

namespace PromoCodeFactory.WebHost;

public static class ValidationDI
{
    public static void ConfigureValidationFailed(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

                return new BadRequestObjectResult(new
                {
                    code = "Validation Failed",
                    errors
                });
            };
        });

    }
}
