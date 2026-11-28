using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Ef;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddEfDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("PromocodeFactoryDb")
                ?? throw new InvalidOperationException("Connection string 'PromocodeFactoryDb' is not configured.");
            return new MongoClient(connectionString);
        });

        services.AddDbContext<PromoCodeFactoryDbContext>((sp, builder) =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            builder.UseMongoDB(client, PromoCodeFactoryDbContext.DatabaseName);
        });

        services.AddScoped<IRepository<Customer>, EfRepository<PromoCodeFactoryDbContext, Customer>>();
        services.AddScoped<IRepository<Preference>, EfRepository<PromoCodeFactoryDbContext, Preference>>();
        services.AddScoped<IRepository<PromoCode>, EfRepository<PromoCodeFactoryDbContext, PromoCode>>();
        return services;
    }
}
