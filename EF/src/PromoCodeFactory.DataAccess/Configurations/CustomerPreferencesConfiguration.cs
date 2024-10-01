using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Configurations
{
    internal class CustomerPreferencesConfiguration : IEntityTypeConfiguration<CustomerPreference>
    {
        public void Configure(EntityTypeBuilder<CustomerPreference> builder)
        {
            builder.HasOne(cp => cp.Customer).WithMany(c => c.CustomersPreferences).HasForeignKey(cp => cp.CustomerId);
            builder.HasOne(cp => cp.Preference).WithMany(p => p.CustomersPreferences).HasForeignKey(cp => cp.PreferenceId);
            builder.HasKey(cp => new { cp.PreferenceId, cp.CustomerId });
            builder.HasData(FakeDataFactory.CustomersPreferences);
        }
    }
}
