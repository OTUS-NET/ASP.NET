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

    //TODO: Добавить DbSet сущности

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<CustomerPromoCode> CustomerPromoCodes { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TODO: Добавить маппинг моделей

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");
            entity.Property(x => x.FirstName).HasMaxLength(50);
            entity.Property(x => x.LastName).HasMaxLength(50);
            entity.Property(x => x.Email).HasMaxLength(256);
            entity.HasMany(x => x.CustomerPromoCodes).WithOne().HasForeignKey(x => x.CustomerId);
            entity.HasMany(x => x.Preferences).WithMany(x => x.Customers);
        });

        modelBuilder.Entity<Preference>(entity =>
        {
            entity.ToTable("Preference");
            entity.Property(x => x.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.ToTable("PromoCode");
            entity.Property(p => p.Code).HasMaxLength(100).IsRequired();
            entity.Property(p => p.ServiceInfo).HasMaxLength(256).IsRequired();
            entity.Property(p => p.PartnerName).HasMaxLength(256).IsRequired();
            entity.HasOne(p => p.PartnerManager).WithMany().IsRequired();
            entity.HasOne(p => p.Preference).WithMany().IsRequired();
            entity.HasMany(p => p.CustomerPromoCodes).WithOne().HasForeignKey(x => x.PromoCodeId);
        });

        modelBuilder.Entity<CustomerPromoCode>(entity =>
        {
            entity.ToTable("CustomerPromoCode");
            entity.HasOne<Customer>().WithMany(c => c.CustomerPromoCodes).HasForeignKey(x => x.CustomerId);
            entity.HasOne<PromoCode>().WithMany(p => p.CustomerPromoCodes).HasForeignKey(x => x.PromoCodeId);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Ignore(e => e.FullName);
            entity.HasOne(e => e.Role).WithMany().IsRequired();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
            entity.Property(r => r.Name).HasMaxLength(100).IsRequired();
            entity.Property(r => r.Description).HasMaxLength(500);
        });

        base.OnModelCreating(modelBuilder);
    }
}
