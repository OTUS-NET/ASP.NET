using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Configuration;

namespace PromoCodeFactory.DataAccess;

public class PromoCodesDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    
    public DbSet<Role> Roles { get; set; }
    
    public DbSet<Customer> Customers { get; set; }
    
    public DbSet<Preference> Preferences { get; set; }
    
    public DbSet<PromoCode> PromoCodes { get; set; }
    
    public DbSet<CustomerPreference> CustomerPreferences { get; set; }

    public PromoCodesDbContext() 
        : base()
    {
    }
    
    public PromoCodesDbContext(DbContextOptions<PromoCodesDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new PreferenceConfiguration());
        modelBuilder.ApplyConfiguration(new PromoCodeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerPreferenceConfiguration());
    }
}