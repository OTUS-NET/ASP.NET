using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    private const string TABLE_NAME = "roles";

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(e => e.Id).HasName($"PK_{TABLE_NAME}");
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(30);

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(255);

        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        //builder.HasData(FakeDataFactory.Roles);
    }
}

