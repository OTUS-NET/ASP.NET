using Microsoft.Extensions.DependencyInjection;

namespace Pcf.ReceivingFromPartner.Integration;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegration(this IServiceCollection services)
    {
        // TODO: зарегистрировать Administration и GivingToCustomer

        return services;
    }
}
