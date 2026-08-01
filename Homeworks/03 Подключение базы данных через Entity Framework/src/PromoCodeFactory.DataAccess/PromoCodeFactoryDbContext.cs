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

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Preference> Preferences => Set<Preference>();

    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();

    public DbSet<CustomerPromoCode> CustomerPromoCodes => Set<CustomerPromoCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(100).IsRequired();
            entity.Property(r => r.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Ignore(e => e.FullName);

            entity.HasOne(e => e.Role)
                .WithMany()
                .IsRequired();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(c => c.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(c => c.LastName).HasMaxLength(50).IsRequired();
            entity.Property(c => c.Email).HasMaxLength(256).IsRequired();
            entity.Ignore(c => c.FullName);

            entity.HasMany(c => c.Preferences)
                .WithMany(p => p.Customers)
                .UsingEntity(j => j.ToTable("CustomerPreference"));

            entity.HasMany(c => c.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey(cpc => cpc.CustomerId);
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

            entity.HasOne(p => p.PartnerManager)
                .WithMany()
                .IsRequired();

            entity.HasOne(p => p.Preference)
                .WithMany()
                .IsRequired();

            entity.HasMany(p => p.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey(cpc => cpc.PromoCodeId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
