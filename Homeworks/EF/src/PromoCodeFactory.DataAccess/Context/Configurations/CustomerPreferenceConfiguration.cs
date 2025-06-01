using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Context.Configurations
{
    public class CustomerPreferenceConfiguration : IEntityTypeConfiguration<CustomerPreference>
    {
        public void Configure(EntityTypeBuilder<CustomerPreference> builder)
        {
            builder.HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

            builder.HasOne(cp => cp.Customer)
                .WithMany(c => c.Preferences)
                .HasForeignKey(cp => cp.CustomerId);

            builder.HasOne(cp => cp.Preference)
                .WithMany(p => p.Customers)
                .HasForeignKey(cp => cp.PreferenceId);

            builder.HasData(FakeDataFactory.CustomerPreferences);
        }
    }
}
