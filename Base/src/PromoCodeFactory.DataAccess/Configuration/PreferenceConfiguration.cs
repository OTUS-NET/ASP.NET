using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.PromoCodeManagement;
using PromoCodeFactory.DataAccess.DataSeeding;
using PromoCodeFactory.DataAccess.Extensions;

namespace PromoCodeFactory.DataAccess.Configuration;

public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
{
    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        builder.ConfigureBaseEntity();
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasData(DatabaseSeeder.GetPreferences());
    }
}