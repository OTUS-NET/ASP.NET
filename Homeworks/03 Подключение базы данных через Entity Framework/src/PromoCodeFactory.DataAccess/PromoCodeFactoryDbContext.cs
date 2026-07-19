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

    public DbSet<PromoCode> PromoCodes { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Preference> Preferences { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ServiceInfo).HasMaxLength(256);
            entity.Property(x => x.PartnerName).HasMaxLength(100);

            entity.HasOne(x => x.PartnerManager)
                .WithMany()
                .IsRequired();

            entity.HasOne(x => x.Preference)
                .WithMany()
                .IsRequired();
        });


        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();

            entity.HasMany(c => c.Preferences)
                .WithMany(p => p.Customers)
                .UsingEntity("CustomerPreference"); 
        });


        modelBuilder.Entity<Preference>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });


        modelBuilder.Entity<CustomerPromoCode>(entity =>
        {

            entity.HasKey(x => new { x.CustomerId, x.PromoCodeId });

            entity.HasOne<Customer>()
                .WithMany(c => c.CustomerPromoCodes)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<PromoCode>()
                .WithMany(p => p.CustomerPromoCodes)
                .HasForeignKey(x => x.PromoCodeId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(x => x.Description).HasMaxLength(500).IsRequired();

        });

        base.OnModelCreating(modelBuilder);
    }
}
