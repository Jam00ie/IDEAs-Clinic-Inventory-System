using IdeasClinicInventory.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeasClinicInventory.Web.Data.Configurations;

/// <summary>
/// Defines component uniqueness and quantity rules in SQL Server.
/// </summary>
public sealed class CatalogItemComponentConfiguration : IEntityTypeConfiguration<CatalogItemComponent>
{
    public void Configure(EntityTypeBuilder<CatalogItemComponent> builder)
    {
        builder.ToTable("CatalogItemComponents", table =>
        {
            table.HasCheckConstraint(
                "CK_CatalogItemComponents_Name_NotBlank",
                "LEN(LTRIM(RTRIM([Name]))) > 0");
            table.HasCheckConstraint(
                "CK_CatalogItemComponents_ExpectedQuantity_Positive",
                "[ExpectedQuantity] > 0");
        });

        builder.HasKey(component => component.Id);
        builder.HasIndex(component => new { component.CatalogItemId, component.Name }).IsUnique();

        builder.Property(component => component.Name).HasMaxLength(200).IsRequired();
        builder.Property(component => component.RowVersion).IsRowVersion();

        builder.HasOne(component => component.CatalogItem)
            .WithMany(item => item.Components)
            .HasForeignKey(component => component.CatalogItemId)
            // Components are part of the catalog definition and have no meaning
            // after their parent catalog item is deleted.
            .OnDelete(DeleteBehavior.Cascade);
    }
}
