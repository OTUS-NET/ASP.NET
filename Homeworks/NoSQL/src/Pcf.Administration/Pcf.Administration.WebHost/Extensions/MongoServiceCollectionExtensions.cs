using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.WebHost.Extensions;

public static class MongoServiceCollectionExtensions
{
    public static void AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(_ => new MongoClient(configuration["MongoDb:ConnectionString"]));
        services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IMongoClient>()
            .GetDatabase(configuration["MongoDb:Database"]));
        services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IMongoDatabase>()
            .GetCollection<Employee>("MongoDb:Employees"));
        services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IMongoDatabase>()
            .GetCollection<Role>("MongoDb:Roles"));
        services.AddScoped(serviceProvider => serviceProvider.GetRequiredService<IMongoClient>()
            .StartSession());
    }
}