using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Infrastructure.EntityFramework.Configurations;

public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
{
    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        BaseEntityConfiguration.ConfigureBaseEntity(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}