using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess;

public class PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Preference> Preferences => Set<Preference>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<CustomerPromoCode> CustomerPromoCodes => Set<CustomerPromoCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Preference>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.HasMany(e => e.Preferences)
                .WithMany(p => p.Customers);
            entity.HasMany(e => e.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey("CustomerId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.ServiceInfo).HasMaxLength(256);
            entity.HasOne<Preference>()
                .WithMany()
                .HasForeignKey(e => e.PreferenceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CustomerPromoCode>(entity =>
        {
            entity.HasOne<PromoCode>()
                .WithMany()
                .HasForeignKey(cpc => cpc.PromoCodeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex("CustomerId", nameof(CustomerPromoCode.PromoCodeId)).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
