using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Infrastructure.EntityFramework.Configurations;

public class CustomerPreferenceConfiguration :IEntityTypeConfiguration<CustomerPreference> 
{
    public void Configure(EntityTypeBuilder<CustomerPreference> builder)
    {
        builder.HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

        builder.HasOne(cp => cp.Customer)
            .WithMany(c => c.CustomerPreferences)
            .HasForeignKey(cp => cp.CustomerId);

        builder.HasOne(cp => cp.Preference)
            .WithMany(p => p.CustomerPreferences)
            .HasForeignKey(cp => cp.PreferenceId);
        builder.HasData(
            new CustomerPreference
            {
                CustomerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                PreferenceId = Guid.Parse("c4bda62e-fc74-4256-a956-4760b3858cbd"),
            });
        builder.HasData(
            new CustomerPreference
            {
                CustomerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0"),
                PreferenceId = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"),
            });
        builder.HasData(
            new CustomerPreference
            {
                CustomerId = Guid.Parse("f766e2bf-340a-46ea-bff3-f1700b435895"),
                PreferenceId = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"),
            });
    }
}