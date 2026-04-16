using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.DataAccess.Configures;
using PromoCodeFactory.DataAccess.Entities;
using Preference = PromoCodeFactory.Core.Domain.PromoCodeManagement.Preference;

namespace PromoCodeFactory.DataAccess;

public class PromoCodeFactoryDbContext : DbContext
{
    public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
        : base(options)
    {
    }

    #region Блок DbSet's
    public required DbSet<Customer> Customers { get; set; }

    public required DbSet<Employee> Employees { get; set; }

    public required DbSet<Preference> Preferences { get; set; }

    public required DbSet<PromoCode> PromoCodes { get; set; }

    public required DbSet<CustomerPromoCode> CustomerPromoCodes { get; set; }

    public required DbSet<Role> Roles { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());

        modelBuilder.ApplyConfiguration(new EmployeeConfigurator());

        modelBuilder.ApplyConfiguration(new PromoCodeConfigurator());

        modelBuilder.ApplyConfiguration(new RoleResponseConfigurator());

        modelBuilder.ApplyConfiguration(new PreferenceConfigurator());         

        base.OnModelCreating(modelBuilder);
    }
}
