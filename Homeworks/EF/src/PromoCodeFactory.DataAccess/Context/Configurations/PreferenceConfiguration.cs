using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Context.Configurations
{
    public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
    {
        public void Configure(EntityTypeBuilder<Preference> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(p => p.Name)
                   .IsUnique();

            builder.HasData(FakeDataFactory.Preferences);
        }
    }
}
