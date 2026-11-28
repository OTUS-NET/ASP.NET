using Microsoft.EntityFrameworkCore;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.DataAccess;

public class PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
    : DbContext(options)
{
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<PartnerPromoCodeLimit> PartnerPromoCodeLimits => Set<PartnerPromoCodeLimit>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Partner>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.HasMany(e => e.PartnerLimits)
                .WithOne(l => l.Partner)
                .HasForeignKey("PartnerId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.ServiceInfo).HasMaxLength(256);
            entity.HasOne(e => e.Partner)
                .WithMany()
                .HasForeignKey("PartnerId")
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
