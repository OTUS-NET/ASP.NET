using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess;

public class PromoCodeFactoryDbContext : DbContext
{
    public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerPromoCode> CustomerPromoCodes => Set<CustomerPromoCode>();
    public DbSet<Preference> Preferences => Set<Preference>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(p => p.FirstName).HasMaxLength(50);
            entity.Property(p => p.LastName).HasMaxLength(50);
            entity.Property(p => p.Email).HasMaxLength(256);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(100);
            entity.Property(p => p.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(p => p.FirstName).HasMaxLength(50);
            entity.Property(p => p.LastName).HasMaxLength(50);
            entity.Property(p => p.Email).HasMaxLength(256);
        });

        modelBuilder.Entity<Preference>()
            .Property(p => p.Name).HasMaxLength(100);

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.Property(p => p.Code).HasMaxLength(100);
            entity.Property(p => p.PartnerName).HasMaxLength(256);
            entity.Property(p => p.ServiceInfo).HasMaxLength(256);

            entity.HasOne(p => p.PartnerManager).WithMany();
        });

        base.OnModelCreating(modelBuilder);
    }
}
