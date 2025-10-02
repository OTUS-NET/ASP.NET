using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.WebHost;

namespace PromoCodesFactory.WebHost.Tests;

public class WebHostFixture : WebApplicationFactory<Program>
{
    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureTestServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<EfContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            var id = Guid.NewGuid().ToString("D");

            services.AddDbContext<EfContext>(options =>
                options.UseInMemoryDatabase(id)
                    .UseSeeding((c, _) => EfContext.SeedData(c))
                    .UseAsyncSeeding((c, _, t) =>
                    {
                        EfContext.SeedData(c);
                        return Task.CompletedTask;
                    }));
        });
        base.ConfigureWebHost(builder);
    }


    internal HttpClient GetClient(bool standalone = false)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EfContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return CreateClient();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        Console.WriteLine("DISPOSE");
        base.Dispose(disposing);
    }
}