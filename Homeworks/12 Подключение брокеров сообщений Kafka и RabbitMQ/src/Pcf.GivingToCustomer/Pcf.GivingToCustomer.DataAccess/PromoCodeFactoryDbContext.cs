using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess;

public class PromoCodeFactoryDbContext : DbContext
{
    public const string DatabaseName = "pcf_givingtocustomer";

    public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
        : base(options)
    {
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Preference> Preferences => Set<Preference>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Preference>(entity =>
        {
            entity.ToCollection("preferences");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToCollection("customers");
            entity.HasIndex(e => e.PreferenceIds);
            entity.OwnsMany(e => e.CustomerPromoCodes, owned =>
            {
                owned.HasKey(e => e.Id);
            });
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.ToCollection("promoCodes");
            entity.HasIndex(e => e.PreferenceId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
