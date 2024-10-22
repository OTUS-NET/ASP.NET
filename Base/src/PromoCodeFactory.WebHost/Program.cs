using PromoCodeFactory.DataAccess.Extensions;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.DataAccess.Repositories.Impl;

namespace PromoCodeFactory.WebHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost
            .UseKestrel((context, option) => { option.Configure(context.Configuration.GetSection("Kestrel")); })
            .ConfigureAppConfiguration((context, configBuilder) =>
            {
                var env = context.HostingEnvironment.EnvironmentName;

                configBuilder.AddJsonFile("appsettings.json");
                configBuilder.AddJsonFile($"appsettings.{env}.json", optional: true);
                configBuilder.AddJsonFile($"Configs/connectionStrings.{env}.json", optional: true);
                configBuilder.AddEnvironmentVariables();
            });

        builder.Services.AddOpenApiDocument(options =>
        {
            options.Title = "PromoCode Factory API Doc";
            options.Version = "1.0";
        });

        builder.Services.AddControllers();

        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        builder.Services.AddPromoCodesDbContext(builder.Configuration.GetConnectionString("PromoCodesDbContext"));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseOpenApi();
        app.UseSwaggerUi(x => { x.DocExpansion = "list"; });

        app.UseHttpsRedirection();
        app.UseRouting();
        app.MapControllers();

        app.Run();
    }
}