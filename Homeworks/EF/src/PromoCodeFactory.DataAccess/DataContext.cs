using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Role> Roles { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<CustomerPreference> CustomerPreferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();

        //https://learn.microsoft.com/ru-ru/ef/core/modeling/data-seeding
        optionsBuilder
        .UseSeeding((dbContext, _) =>
        {
            var context = (DataContext)dbContext;
            var logger = context.GetService<ILogger<DataContext>>();

        })
        .UseAsyncSeeding(async (dbContext, _, сancellationToken) =>
        {
            var context = (DataContext)dbContext;
            var logger = context.GetService<ILogger<DataContext>>();
        });

        base.OnConfiguring(optionsBuilder);

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
