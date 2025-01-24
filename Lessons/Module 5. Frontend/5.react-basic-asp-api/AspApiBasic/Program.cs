using AspApiBasic.Extensions;

namespace AspApiBasic;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add CORS Policies here
        builder.Services.AddCorsPolicy();
        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddCustomSwagger();
        builder.Services.AddDefaultSpaMiddleware();
        var app = builder.Build();
        
        // Use CORS Middleware here
        app.UseCors(ApiCorsPolicies.AllowAllOrigins);
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        //SPA Middlewares
        app.UseDefaultSpaMiddleware();
       
        app.Run();
    }
}