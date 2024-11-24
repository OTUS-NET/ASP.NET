using DirectoryOfPreferences.Application.Abstractions;
using DirectoryOfPreferences.Application.Implementations;

namespace DirectoryOfPreferences
{
    public static class Registrar
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPreferenceService, PreferenceService>();
            return services;
        }
    }
}
