using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Integration.Administration;
using Pcf.ReceivingFromPartner.Integration.GivingToCustomer;

namespace Pcf.ReceivingFromPartner.Integration;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegration(this IServiceCollection services)
    {
        services.AddHttpClient<IGivingToCustomerGateway, GivingToCustomerGateway>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var url = configuration.GetValue<string>("IntegrationSettings:GivingToCustomerApiUrl")
                ?? throw new InvalidOperationException("IntegrationSettings:GivingToCustomerApiUrl is required");
            client.BaseAddress = new Uri(url);
        });

        services.AddHttpClient<IAdministrationGateway, AdministrationGateway>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var url = configuration.GetValue<string>("IntegrationSettings:AdministrationApiUrl")
                ?? throw new InvalidOperationException("IntegrationSettings:AdministrationApiUrl is required");
            client.BaseAddress = new Uri(url);
        });

        return services;
    }
}
