using System.Linq;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess;

public class EfContext(DbContextOptions<EfContext> options) : DbContext(options)
{

    public static void SeedData(DbContext context)
    {
        context.Set<Role>().AddRange(FakeDataFactory.Roles);
        context.Set<Preference>().AddRange(FakeDataFactory.Preferences);
        context.SaveChanges();
        
        var customers = FakeDataFactory.Customers.ToList();
        foreach (var customer in customers)
        {
             customer.Preferences = 
                 context
                     .Set<Preference>()
                     .AsEnumerable()
                     .Where(r => customer.Preferences.Any(c => c.Id == r.Id))
                     .ToList();
        }
        context.Set<Customer>().AddRange(customers);
        
        var employees = FakeDataFactory.Employees.ToList();
        foreach (var employee in employees)
        {
            employee.Role = context.Set<Role>().AsEnumerable().Single(r => r.Id == employee.Role.Id);
        }
        context.Set<Employee>().AddRange(employees);
        
        context.SaveChanges();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //NOTE: MaxLength для строк указана через атрибуты в сущностях
        modelBuilder
            .Entity<Role>()
            .HasKey(x => x.Id);

        modelBuilder
            .Entity<Preference>()
            .HasKey(x => x.Id);
        
        modelBuilder
            .Entity<PromoCode>()
            .HasOne(x => x.Preference);
        
        modelBuilder
            .Entity<Employee>()
            .HasOne(x => x.Role)
            .WithMany(x => x.Employees);
        
        modelBuilder
            .Entity<Employee>()
            .Navigation(x => x.Role)
            .AutoInclude();

        modelBuilder
            .Entity<Customer>()
            .HasMany(x => x.Preferences)
            .WithMany(x => x.Customers)
            .UsingEntity(
                nameof(CustomerPreference),
                l => l.HasOne(typeof(Customer)).WithMany().HasForeignKey("CustomersId").IsRequired(),
                r => r.HasOne(typeof(Preference)).WithMany().HasForeignKey("PreferencesId").IsRequired(),
                j => j.HasKey("CustomersId", "PreferencesId")
            );

        modelBuilder
            .Entity<Customer>()
            .HasMany(x => x.PromoCodes)
            .WithOne(x => x.Customer)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder
            .Entity<Customer>()
            .Navigation(x => x.PromoCodes)
            .AutoInclude();
        
        modelBuilder
            .Entity<Customer>()
            .Navigation(x => x.Preferences)
            .AutoInclude();

        base.OnModelCreating(modelBuilder);
    }
}