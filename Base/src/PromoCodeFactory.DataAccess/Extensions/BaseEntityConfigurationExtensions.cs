using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core;

namespace PromoCodeFactory.DataAccess.Extensions;

public static class BaseEntityConfigurationExtensions
{
    public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasConversion<DateTime>()
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .HasConversion<DateTime>();
    }
}