using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess;

public class DataContext
    : DbContext
{
    public DbSet<PromoCode> PromoCodes { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Preference> Preferences { get; set; }

    public DataContext()
    {

    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.Entity<CustomerPreference>()
        //.HasKey(bc => bc.Id);
        //modelBuilder.Entity<Customer>(builder =>
        //{
        //    builder.HasMany(b => b.Preferences)
        //    .WithOne()
        //    .HasForeignKey(c => c.Id);
        //    builder.HasMany(b => b.PromoCodes)
        //    .WithOne()
        //    .HasForeignKey(c => c.Id);
        //});
    }
}