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

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<CustomerPromoCode> CustomerPromoCodes { get; set; } 
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Ignore(e => e.FullName);
            entity.HasOne(e => e.Role).WithMany().IsRequired();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(100).IsRequired();
            entity.Property(r => r.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(c => c.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(c => c.LastName).HasMaxLength(50).IsRequired();
            entity.Property(c => c.Email).HasMaxLength(256).IsRequired();
            entity.Ignore(c => c.FullName);

            entity.HasMany(c => c.Preferences)
                .WithMany(p => p.Customers);

            entity.HasMany(c => c.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey(x => x.CustomerId);
        });

        modelBuilder.Entity<Preference>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.Property(p => p.Code).HasMaxLength(100).IsRequired();
            entity.Property(p => p.ServiceInfo).HasMaxLength(256).IsRequired();
            entity.Property(p => p.PartnerName).HasMaxLength(256).IsRequired();

            entity.HasOne(p => p.PartnerManager).WithMany().IsRequired();
            entity.HasOne(p => p.Preference).WithMany().IsRequired();

            entity.HasMany(p => p.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey(x => x.PromoCodeId);
        });

        modelBuilder.Entity<CustomerPromoCode>(entity =>
        {
            entity.HasOne<Customer>()
                .WithMany(c => c.CustomerPromoCodes)
                .HasForeignKey(x => x.CustomerId);

            entity.HasOne<PromoCode>()
                .WithMany(p => p.CustomerPromoCodes)
                .HasForeignKey(x => x.PromoCodeId);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}
