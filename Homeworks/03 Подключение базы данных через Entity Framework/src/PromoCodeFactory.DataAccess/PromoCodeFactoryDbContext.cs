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
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public DbSet<PromoCode> PromoCodes { get; set; }

    public DbSet<Preference> Preferences { get; set; }

    public DbSet<CustomerPromoCode> CustomerPromoCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TODO: Добавить маппинг моделей
        modelBuilder.Entity<Employee>()
            .Property(u => u.FirstName)
            .HasMaxLength(50);

        modelBuilder.Entity<Employee>()
            .Property(u => u.LastName)
            .HasMaxLength(50);

        modelBuilder.Entity<Employee>()
            .Property(u => u.Email)
            .HasMaxLength(256);

        modelBuilder.Entity<Customer>()
            .Property(u => u.FirstName)
            .HasMaxLength(50);

        modelBuilder.Entity<Customer>()
            .Property(u => u.LastName)
            .HasMaxLength(50);

        modelBuilder.Entity<Customer>()
            .Property(u => u.Email)
            .HasMaxLength(256);

        modelBuilder.Entity<Role>()
            .Property(u => u.Name)
            .HasMaxLength(100);

        modelBuilder.Entity<Role>()
            .Property(u => u.Description)
            .HasMaxLength(500);


        modelBuilder.Entity<PromoCode>()
        .Property(u => u.Code)
        .HasMaxLength(256);

        modelBuilder.Entity<PromoCode>()
            .Property(u => u.ServiceInfo)
            .HasMaxLength(256);

        modelBuilder.Entity<PromoCode>()
            .Property(u => u.PartnerName)
            .HasMaxLength(256);

        base.OnModelCreating(modelBuilder);
    }
}
