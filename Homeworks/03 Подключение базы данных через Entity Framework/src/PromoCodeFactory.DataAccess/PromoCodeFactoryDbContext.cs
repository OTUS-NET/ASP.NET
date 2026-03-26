using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
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

    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Customer> Cusomers { get; set; } = null!;
    public DbSet<CustomerPromoCode> CustomerPromoCodes { get; set; } = null!;
    public DbSet<Preference> Preferences { get; set; } = null!;
    public DbSet<PromoCode> PromoCodes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ограничения
        modelBuilder.Entity<Employee>(e =>
        {
            e.Property(e => e.FirstName).HasMaxLength(50);
            e.Property(e => e.LastName).HasMaxLength(50);
            e.Property(e => e.Email).HasMaxLength(256);
        });

        modelBuilder.Entity<Role>(r =>
        {
            r.Property(r => r.Name).HasMaxLength(100);
            r.Property(r => r.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Customer>(c =>
        {
            c.Property(c => c.FirstName).HasMaxLength(50);
            c.Property(c => c.LastName).HasMaxLength(50);
            c.Property(c => c.Email).HasMaxLength(256);
        });

        modelBuilder.Entity<PromoCode>(pc =>
        {
            pc.Property(pc => pc.Code).HasMaxLength(256);
            pc.Property(pc => pc.ServiceInfo).HasMaxLength(256);
            pc.Property(pc => pc.PartnerName).HasMaxLength(256);
        });

        modelBuilder.Entity<Preference>().Property(p => p.Name).HasMaxLength(100);

        // Ссылки
        modelBuilder.Entity<Employee>().HasOne(e => e.Role);
        modelBuilder.Entity<PromoCode>().HasOne(pc => pc.PartnerManager);
        modelBuilder.Entity<PromoCode>().HasOne(pc => pc.Preference);
        modelBuilder.Entity<PromoCode>()
            .HasMany(pc => pc.CustomerPromoCodes)
            .WithOne()
            .HasForeignKey(cpc => cpc.PromoCodeId);
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.CustomerPromoCodes)
            .WithOne()
            .HasForeignKey(cpc => cpc.CustomerId);
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Preferences)
            .WithMany(p => p.Customers)
            .UsingEntity("CustomerPreference",
                p => p.HasOne(typeof(Preference))
                .WithMany()
                .HasForeignKey("PreferenceId"),
                c => c.HasOne(typeof(Customer))
                .WithMany()
                .HasForeignKey("CustomerId"),
                j =>
                {
                    j.Property<Guid>("CustomerId");
                    j.Property<Guid>("PreferenceId");
                    j.HasKey("CustomerId", "PreferenceId");
                });
        base.OnModelCreating(modelBuilder);
    }
}
