using System.Linq;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess;

public class EfContext(DbContextOptions<EfContext> options) : DbContext(options)
{
    /// <inheritdoc />
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     base.OnConfiguring(optionsBuilder);
    //     
    //     Database.EnsureDeleted();
    //     Database.EnsureCreated();
    //     
    // }

    public static void SeedData(DbContext context)
    {
        context.Set<Role>().AddRange(FakeDataFactory.Roles);
        context.Set<Preference>().AddRange(FakeDataFactory.Preferences);
        context.SaveChanges();
        
        var customers = FakeDataFactory.Customers.ToList();
        
        // foreach (var customer in customers)
        // {
        //     var prefs = customer.Preferences.ToList();
        //     customer.Preferences = context.Set<Preference>().Where(x => prefs.Any(z => z.Id == x.Id)).ToList();
        // }
        context.Set<Customer>().AddRange(customers);
        
        var employees = FakeDataFactory.Employees.ToList();
        //var roles = context.Set<Role>().ToList();
        foreach (var employee in employees)
        {
            employee.Role = context.Set<Role>().Single(r => r.Id == employee.Role.Id);
        }
        context.Set<Employee>().AddRange(employees);

        // var rolesToAdd = FakeDataFactory.Roles
        //     .Where(x => context.Set<Role>().All(z => z.Id != x.Id))
        //     .ToList();
        
        
        context.SaveChanges();
    }


    //Database.EnsureDeleted();
    //Database.EnsureCreated();

    // <inheritdoc />
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlite("Data Source=PromoCodeFactory.db");
    //     base.OnConfiguring(optionsBuilder);
    // }

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

        modelBuilder
            .Entity<PromoCode>()
            .HasOne(x => x.Preference);

        modelBuilder
            .Entity<Employee>()
            .HasOne(x => x.Role)
            .WithMany(x => x.Employees)
            ;//.HasForeignKey(x=> x.Role);

        // modelBuilder
        //     .Entity<Employee>()
        //     .Property<Guid>("RoleId");
        //
        // modelBuilder
        //     .Entity<Employee>()
        //     .HasOne(x => x.Role)
        //     .WithMany()
        //     .HasForeignKey("RoleId");

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


        // modelBuilder
        //     .Entity<Role>()
        //     .HasData(FakeDataFactory.Roles);
        //
        // modelBuilder
        //     .Entity<Preference>()
        //     .HasData(FakeDataFactory.Preferences);
        //
        // modelBuilder
        //     .Entity<Customer>()
        //     .HasData(FakeDataFactory.Customers);
        //
        // modelBuilder
        //     .Entity<Employee>()
        //     .HasData(FakeDataFactory.Employees);

        base.OnModelCreating(modelBuilder);
    }
}