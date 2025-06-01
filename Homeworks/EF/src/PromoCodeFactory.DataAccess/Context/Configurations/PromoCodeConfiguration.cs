using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Context.Configurations
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.HasOne(pc => pc.Preference)
                .WithMany(p => p.PromoCodes)
                .HasForeignKey(pc => pc.PreferenceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pc => pc.Customer)
                .WithMany(c => c.PromoCodes)
                .HasForeignKey(pc => pc.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(pc => pc.Code)
                .IsRequired()
                .HasMaxLength(12);

            builder.HasIndex(pc => pc.Code)
                   .IsUnique();

            builder.Property(pc => pc.ServiceInfo)
                .HasMaxLength(255);

            builder.Property(pc => pc.PartnerName)
                .HasMaxLength(100);

            builder.HasData(FakeDataFactory.PromoCodes);
        }
    }
}
