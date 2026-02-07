using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Configurations
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder
                .HasOne(p => p.PartnerManager)
                .WithMany(e => e.Promocodes)
                .HasForeignKey(p => p.PartnerManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasOne(p => p.Customer)
                .WithMany(c => c.Promocodes)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(p => p.Preference)
                .WithMany(pr => pr.Promocodes)
                .HasForeignKey(p => p.PreferenceId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.Code).HasMaxLength(20);
            builder.Property(x => x.ServiceInfo).HasMaxLength(1000);
            builder.Property(x => x.PartnerName).HasMaxLength(500);
        }
    }
}
