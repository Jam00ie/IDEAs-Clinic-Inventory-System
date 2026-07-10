using IdeasClinicInventory.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeasClinicInventory.Web.Data.Configurations;

/// <summary>
/// Defines SQL relationships and constraints for individually tracked units.
/// </summary>
public sealed class TrackedUnitConfiguration : IEntityTypeConfiguration<TrackedUnit>
{
    public void Configure(EntityTypeBuilder<TrackedUnit> builder)
    {
        builder.ToTable("TrackedUnits", table =>
            table.HasCheckConstraint(
                "CK_TrackedUnits_Identifier_NotBlank",
                "LEN(LTRIM(RTRIM([Identifier]))) > 0"));

        builder.HasKey(unit => unit.Id);

        // Identifiers need only be unique within a catalog item. Different item
        // types may legitimately use the same label, such as "1" or "A1".
        builder.HasIndex(unit => new { unit.CatalogItemId, unit.Identifier }).IsUnique();

        builder.Property(unit => unit.Identifier).HasMaxLength(100).IsRequired();
        builder.Property(unit => unit.Notes).HasMaxLength(4_000);
        builder.Property(unit => unit.Status).HasConversion<string>().HasMaxLength(40);
        builder.Property(unit => unit.RowVersion).IsRowVersion();

        builder.HasOne(unit => unit.CatalogItem)
            .WithMany(item => item.TrackedUnits)
            .HasForeignKey(unit => unit.CatalogItemId)
            // A catalog item with physical units must not disappear underneath them.
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(unit => unit.HomeLocation)
            .WithMany(location => location.TrackedUnits)
            .HasForeignKey(unit => unit.HomeLocationId)
            // Moving/deleting a location must be explicit while units still use it.
            .OnDelete(DeleteBehavior.Restrict);
    }
}
