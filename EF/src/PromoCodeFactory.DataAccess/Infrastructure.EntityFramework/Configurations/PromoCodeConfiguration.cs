using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Infrastructure.EntityFramework.Configurations;

public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        BaseEntityConfiguration.ConfigureBaseEntity(builder);

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ServiceInfo)
            .HasColumnName("service_info")
            .HasMaxLength(200);

        builder.Property(x => x.BeginDate)
            .HasColumnName("begin_date")
            .IsRequired();

        builder.Property(x => x.EndDate)
            .HasColumnName("end_date")
            .IsRequired();

        builder.Property(x => x.PartnerName)
            .HasColumnName("partner_name")
            .HasMaxLength(50);

        builder.HasOne(x => x.PartnerManager)
            .WithMany()
            .HasForeignKey(x => x.PartnerManagerId);

        builder.HasOne(x => x.Preference)
            .WithMany()
            .HasForeignKey(x => x.PreferenceId);
    }
}