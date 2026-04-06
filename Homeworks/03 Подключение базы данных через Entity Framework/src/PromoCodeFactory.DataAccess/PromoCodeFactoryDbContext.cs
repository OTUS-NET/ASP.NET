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
    public DbSet<Preference> Preferences => Set<Preference>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<CustomerPromoCode> CustomerPromoCodes => Set<CustomerPromoCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Role
        modelBuilder.Entity<Role>(r =>
        {
            r.HasKey(x => x.Id);

            r.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            r.Property(x => x.Description)
                .HasMaxLength(500);
        });

        // Employee
        modelBuilder.Entity<Employee>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            e.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            e.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            e.HasOne(x => x.Role)
                .WithMany()
                .IsRequired();
        });

        // Preference
        modelBuilder.Entity<Preference>(p =>
        {
            p.HasKey(x => x.Id);

            p.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        // Customer
        modelBuilder.Entity<Customer>(c =>
        {
            c.HasKey(x => x.Id);

            c.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            c.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            c.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            c.HasMany(x => x.Preferences)
                .WithMany(x => x.Customers)
                .UsingEntity(j =>
                {
                    j.ToTable("CustomerPreferences");
                });

            c.HasMany(x => x.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PromoCode
        modelBuilder.Entity<PromoCode>(p =>
        {
            p.HasKey(x => x.Id);

            p.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(256);

            p.Property(x => x.ServiceInfo)
                .IsRequired()
                .HasMaxLength(256);

            p.Property(x => x.PartnerName)
                .IsRequired()
                .HasMaxLength(256);

            p.Property(x => x.BeginDate).IsRequired();
            p.Property(x => x.EndDate).IsRequired();

            p.HasOne(x => x.PartnerManager)
                .WithMany()
                .IsRequired();

            p.HasOne(x => x.Preference)
                .WithMany()
                .IsRequired();

            p.HasMany(x => x.CustomerPromoCodes)
                .WithOne()
                .HasForeignKey(x => x.PromoCodeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CustomerPromoCode (таблица выдачи/использования промокодов клиентами)
        modelBuilder.Entity<CustomerPromoCode>(cpc =>
        {
            cpc.HasKey(x => x.Id);

            cpc.Property(x => x.CreatedAt)
                .IsRequired();

            cpc.Property(x => x.AppliedAt);

            // Индексы
            cpc.HasIndex(x => x.CustomerId);
            cpc.HasIndex(x => x.PromoCodeId);

            // Уникальность промро
            cpc.HasIndex(x => new { x.CustomerId, x.PromoCodeId })
                .IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
