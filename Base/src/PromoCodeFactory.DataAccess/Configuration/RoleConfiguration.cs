using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Administration;
using PromoCodeFactory.DataAccess.DataSeeding;
using PromoCodeFactory.DataAccess.Extensions;

namespace PromoCodeFactory.DataAccess.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ConfigureBaseEntity();
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(200);

        builder.HasData(DatabaseSeeder.GetRoles());
    }
}