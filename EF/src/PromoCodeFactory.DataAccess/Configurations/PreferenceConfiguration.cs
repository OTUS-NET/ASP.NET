using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Configurations
{
    internal class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
    {
        public void Configure(EntityTypeBuilder<Preference> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(25).IsRequired();

            builder.HasMany(p => p.CustomersPreferences).WithOne(cp => cp.Preference).HasForeignKey(cp => cp.PreferenceId);
            builder.HasData(FakeDataFactory.Preferences);
        }
    }
}
