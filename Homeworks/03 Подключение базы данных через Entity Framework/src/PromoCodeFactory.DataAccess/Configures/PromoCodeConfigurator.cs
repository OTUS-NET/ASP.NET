using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.DataAccess.Entities;

namespace PromoCodeFactory.DataAccess.Configures;

internal class PromoCodeConfigurator : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.ToTable("PromoCodes");

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ServiceInfo)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.PartnerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(x => x.PartnerManager)
            .WithMany()
            .HasForeignKey(x => x.PartnerManagerId);

        builder.HasOne(x => x.Preference)
            .WithMany()
            .HasForeignKey(x => x.PreferenceId);
    }
}
