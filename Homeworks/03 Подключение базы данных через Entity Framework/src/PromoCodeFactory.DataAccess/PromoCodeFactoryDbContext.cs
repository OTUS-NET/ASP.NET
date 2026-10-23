using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess;

public class PromoCodeFactoryDbContext : DbContext
{
    public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
        : base(options)
    {
    }
    public DbSet<BaseEntity> baseEntities { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get ; set; }
    public DbSet<Customer>  Customers { get ; set; }
    public DbSet<CustomerPromoCode>  customerPromoCodes { get ; set; }
    public DbSet<Preference> Preferences { get ; set; }
    public DbSet<PromoCode> PromoCodes { get ; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().Property(p => p.FirstName).HasMaxLength(50);
        modelBuilder.Entity<Customer>().Property(p => p.LastName).HasMaxLength(50);
        modelBuilder.Entity<Customer>().Property(p => p.Email).HasMaxLength(256);
        modelBuilder.Entity<Preference>().Property(p => p.Name).HasMaxLength(100);
        modelBuilder.Entity<PromoCode>().Property(p => p.Code).HasMaxLength(256);
        modelBuilder.Entity<PromoCode>().Property(p => p.ServiceInfo).HasMaxLength(256);
        modelBuilder.Entity<PromoCode>().Property(p => p.PartnerName).HasMaxLength(100);
        base.OnModelCreating(modelBuilder);
    }
}
