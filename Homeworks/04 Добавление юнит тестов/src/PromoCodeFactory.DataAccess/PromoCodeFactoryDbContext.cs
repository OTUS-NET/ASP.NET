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
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<PartnerPromoCodeLimit> PartnerPromoCodeLimits => Set<PartnerPromoCodeLimit>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<Preference> Preferences => Set<Preference>();
    public DbSet<CustomerPromoCode> CustomerPromoCodes => Set<CustomerPromoCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Restrict);
        });

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
                .HasForeignKey(cpc => cpc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey("ManagerId")
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.PartnerLimits)
                .WithOne(l => l.Partner)
                .HasForeignKey("PartnerId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.ServiceInfo).HasMaxLength(256);
            entity.HasOne(e => e.Partner)
                .WithMany()
                .HasForeignKey("PartnerId")
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Preference)
                .WithMany()
                .HasForeignKey("PreferenceId")
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey(cpc => cpc.PromoCodeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CustomerPromoCode>(entity =>
        {
            entity.HasIndex(cpc => new { cpc.CustomerId, cpc.PromoCodeId }).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
