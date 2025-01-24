using Microsoft.Extensions.FileProviders;

namespace AspApiBasic.Extensions;

public static class SpaExtension
{
    public static void AddDefaultSpaMiddleware(this IServiceCollection services)
    {
        services.AddSpaStaticFiles(c => { c.RootPath = "ClientApp"; });
    }

    public static void UseDefaultSpaMiddleware(this WebApplication app)
    {
        var clientAppPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp");
        if (Directory.Exists(clientAppPath))
        {
            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                
               /* if(app.Environment.IsDevelopment())
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:5272");*/ //пример проксирования к серверу фронтенда
            });
        }
    }


    public static void UseCustomSpaMiddleware(this IApplicationBuilder app)
    {
        var clientAppPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp");
        if (Directory.Exists(clientAppPath))
        {
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(clientAppPath),
                RequestPath = "" // Указываем, что статические файлы будут доступны по корневому пути
            });

            // Если запрос не обработан API, перенаправляем на index.html (для SPA)
            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync(Path.Combine(clientAppPath, "index.html"));
                }
                else
                {
                    await next();
                }
            });
        }
    }
}