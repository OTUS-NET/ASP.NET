using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Configurations;

public class PromocodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    private const string TABLE_NAME = "promo_codes";

    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.HasKey(e => e.Id).HasName($"PK_{TABLE_NAME}");
        builder.Property(e => e.Id).HasColumnName("id").IsRequired();

        builder.Property(p => p.Code).HasColumnName("code").IsRequired().HasMaxLength(50);

        builder.Property(p => p.ServiceInfo).HasColumnName("service_info").HasMaxLength(500);
        builder.Property(p => p.BeginDate).HasColumnName("begin_date").IsRequired();
        builder.Property(p => p.EndDate).HasColumnName("end_date").IsRequired();
        builder.Property(p => p.PartnerName).HasColumnName("partner_name").IsRequired().HasMaxLength(100);
        builder.Property(p => p.PreferenceId).HasColumnName("preference_id");
        builder.Property(p => p.CustomerId).HasColumnName("customer_id");

        // Связи
        builder.HasOne(p => p.PartnerManager)
            .WithMany(e => e.PromoCodes)
            .HasForeignKey(p => p.PartnerManagerId);

        builder.HasOne(p => p.Preference)
            .WithMany(pr => pr.PromoCodes)
            .HasForeignKey(p => p.PreferenceId);

        //builder.HasData(FakeDataFactory.Pr);
    }
}

