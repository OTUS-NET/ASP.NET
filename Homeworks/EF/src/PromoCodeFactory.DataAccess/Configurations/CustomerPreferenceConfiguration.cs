using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Configurations;

public class CustomerPreferenceConfiguration : IEntityTypeConfiguration<CustomerPreference>
{
    private const string TABLE_NAME = "customer_preference";

    public void Configure(EntityTypeBuilder<CustomerPreference> builder)
    {
        builder.HasKey(e => e.Id).HasName($"PK_{TABLE_NAME}");
        builder.Property(e => e.Id).HasColumnName("id").IsRequired();

        builder.Property(u => u.CustomerId).IsRequired().HasColumnName("customer_id");
        builder.Property(u => u.PreferenceId).IsRequired().HasColumnName("preference_id");

        builder.HasOne(cp => cp.Customer)
              .WithMany(c => c.CustomerPreferences)
              .HasForeignKey(cp => cp.CustomerId);

        builder.HasOne(cp => cp.Preference)
               .WithMany(p => p.CustomerPreferences)
               .HasForeignKey(cp => cp.PreferenceId);
    }
}

