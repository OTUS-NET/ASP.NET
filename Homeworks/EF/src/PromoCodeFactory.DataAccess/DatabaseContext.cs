using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // modelBuilder.Entity<Role>().HasKey(r => r.Id);
        // modelBuilder.Entity<Employee>().HasKey(e => e.Id);
        // modelBuilder.Entity<PromoCode>().HasKey(pc => pc.Id);
        // modelBuilder.Entity<Customer>().HasKey(c => c.Id);
        // modelBuilder.Entity<Preference>().HasKey(p => p.Id);
        
        modelBuilder.Entity<Role>().Property(r => r.Name).HasMaxLength(100);

        modelBuilder.Entity<Employee>().Property(e => e.FirstName).HasMaxLength(100);
        modelBuilder.Entity<Employee>().Property(e => e.LastName).HasMaxLength(100);
        modelBuilder.Entity<Employee>().Property(e => e.Email).HasMaxLength(100);
        modelBuilder.Entity<Employee>().HasOne(e => e.Role);
       
        modelBuilder.Entity<PromoCode>().Property(pc => pc.Code).HasMaxLength(20);
        modelBuilder.Entity<PromoCode>().Property(pc => pc.PartnerName).HasMaxLength(100);
        modelBuilder.Entity<PromoCode>().Property(pc => pc.ServiceInfo).HasMaxLength(200);
        modelBuilder.Entity<PromoCode>().HasOne(pc => pc.PartnerManager);
        modelBuilder.Entity<PromoCode>().HasOne(pc => pc.Preference);

        modelBuilder.Entity<Customer>().Property(c => c.FirstName).HasMaxLength(100);
        modelBuilder.Entity<Customer>().Property(c => c.LastName).HasMaxLength(100);
        modelBuilder.Entity<Customer>().Property(c => c.Email).HasMaxLength(100);
        modelBuilder.Entity<Customer>().HasMany(c => c.PromoCodes);
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Preferences)
            .WithMany(p => p.Customers)
            .UsingEntity(e => e.ToTable("CustomerPreference"));
        
        modelBuilder.Entity<Preference>().Property(p => p.Name).HasMaxLength(100);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }
}