using System.Collections;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace PromoCodeFactory.DataAccess;

public class EfContext : DbContext
{
    //Employee, Roles, Customer,Preference и PromoCode
    public DbSet<Employee>  Employees { get; set; }
    public DbSet<Role>  Roles { get; set; }
    public DbSet<Customer>  Customers { get; set; }
    public DbSet<Preference>  Preferences { get; set; }
    public DbSet<PromoCode>  PromoCodes { get; set; }

    // public EfContext(DbContextOptions<EfContext> options)
    // {
    //     
    // }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=PromoCodeFactory.db");
        base.OnConfiguring(optionsBuilder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //PromoCode имеет ссылку на Preference
        //Employee имеет ссылку на Role.
        //Customer имеет набор Preference,
        //но так как Preference - это общий справочник и сущности связаны через Many-to-many,
        //то нужно сделать маппинг через сущность CustomerPreference.
        //Строковые поля должны иметь ограничения на MaxLength.
        //Связь Customer и Promocode реализовать через One-To-Many,
        //будем считать, что в данном примере
        //промокод может быть выдан только одному клиенту из базы.
        
        //modelBuilder
        //    .Entity<Employee>()
            //.HasData(FakeDataFactory.Employees);

        modelBuilder
            .Entity<PromoCode>()
            .HasOne(x => x.Preference);
        
        modelBuilder
            .Entity<Employee>()
            .HasOne(x => x.Role)
            .WithMany();
        
        modelBuilder
            .Entity<Customer>()
            .HasMany(x => x.Preferences)
            .WithMany(x => x.Customers)
            .UsingEntity(
                "CustomerPreference",
                l => l.HasOne(typeof(Customer)).WithMany().HasForeignKey("CustomersId").IsRequired(),
                r => r.HasOne(typeof(Preference)).WithMany().HasForeignKey("PreferencesId").IsRequired(), 
                j => j.HasKey("CustomersId", "PreferencesId")
                )
            ;
        
        modelBuilder
            .Entity<Customer>()
            .HasMany(x => x.PromoCodes)
            .WithOne(x => x.Customer);
        
        // modelBuilder
        //     .Entity<Preference>()
        //     .HasMany(x => x.Id);
            
        base.OnModelCreating(modelBuilder);
    }
}