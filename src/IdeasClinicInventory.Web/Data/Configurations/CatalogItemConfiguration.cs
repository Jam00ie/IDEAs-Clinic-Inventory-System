using IdeasClinicInventory.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeasClinicInventory.Web.Data.Configurations;

/// <summary>
/// Contains database-specific rules so the domain model is not coupled to SQL Server APIs.
/// </summary>
public sealed class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("CatalogItems", table =>
        {
            table.HasCheckConstraint(
                "CK_CatalogItems_Name_NotBlank",
                "LEN(LTRIM(RTRIM([Name]))) > 0");
            table.HasCheckConstraint(
                "CK_CatalogItems_QuantityReservedForAdmins_NonNegative",
                "[QuantityReservedForAdmins] >= 0");
        });

        builder.HasKey(item => item.Id);
        builder.HasIndex(item => item.Name).IsUnique();

        builder.Property(item => item.Name).HasMaxLength(200).IsRequired();
        builder.Property(item => item.Category).HasMaxLength(100);
        builder.Property(item => item.UnitOfMeasure).HasMaxLength(50);
        builder.Property(item => item.ReferenceImagePath).HasMaxLength(2_048);
        builder.Property(item => item.Notes).HasMaxLength(4_000);

        // Storing enum names keeps SQL data readable and avoids changing meanings if
        // enum members are reordered later.
        builder.Property(item => item.AllowedUseType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(item => item.RowVersion).IsRowVersion();
    }
}
