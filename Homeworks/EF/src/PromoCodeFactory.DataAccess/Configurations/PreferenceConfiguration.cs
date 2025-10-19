using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Configurations;

public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
{
    private const string TABLE_NAME = "preferences";

    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        builder.HasKey(e => e.Id).HasName($"PK_{TABLE_NAME}");
        builder.Property(e => e.Id).HasColumnName("id").IsRequired();

        builder.Property(u => u.Name).IsRequired().HasColumnName("name");

        builder.HasMany(p => p.CustomerPreferences)
               .WithOne(cp => cp.Preference)
               .HasForeignKey(cp => cp.PreferenceId);
    }
}

