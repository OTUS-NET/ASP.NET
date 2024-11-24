using DirectoryOfPreferences.Application.Abstractions;
using DirectoryOfPreferences.Application.Implementations;
using DirectoryOfPreferences.Domain.Abstractions;
using DirectoryOfPreferences.Domain.Entity;
using DirectoryOfPreferences.Infrastructure.EntityFramework;
using DirectoryOfPreferences.Infrastructure.Repositories.Implementations;
using DirectoryOfPreferences.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DirectoryOfPreferences
{
    public static class Registrar
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Preference, Guid>, EFRepository<Preference, Guid>>();
            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPreferenceService, PreferenceService>();
            return services;
        }
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreatePreferenceValidator>();
            return services;
        }
        public static IServiceCollection AddApplicationDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Postgres"),
                optionsBuilder => optionsBuilder.MigrationsAssembly("DirectoryOfPreferences.Infrastructure.EntityFramework"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            return services;
        }
    }
}
