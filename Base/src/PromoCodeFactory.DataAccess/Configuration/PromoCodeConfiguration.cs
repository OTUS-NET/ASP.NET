using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Extensions;

namespace PromoCodeFactory.DataAccess.Configuration;

public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.ConfigureBaseEntity();
        
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

        builder.Property(x => x.CustomerId)
            .HasColumnName("customer_id")
            .HasConversion<Guid>();
        
        builder.Property(x => x.PartnerManagerId)
            .HasColumnName("partner_manager_id")
            .HasConversion<Guid>();
        
        builder.Property(x => x.PreferenceId)
            .HasColumnName("preference_id")
            .HasConversion<Guid>();
            
        builder.HasOne(x => x.PartnerManager)
            .WithMany()
            .HasForeignKey(x => x.PartnerManagerId);

        builder.HasOne(x => x.Preference)
            .WithMany()
            .HasForeignKey(x => x.PreferenceId);
        
        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId);
    }
}