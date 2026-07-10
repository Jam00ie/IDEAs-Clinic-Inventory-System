using IdeasClinicInventory.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeasClinicInventory.Web.Data.Configurations;

/// <summary>
/// Defines the one-stock-bucket-per-catalog-item-and-location rule.
/// </summary>
public sealed class UntrackedUnitConfiguration : IEntityTypeConfiguration<UntrackedUnit>
{
    public void Configure(EntityTypeBuilder<UntrackedUnit> builder)
    {
        builder.ToTable("UntrackedUnits", table =>
            table.HasCheckConstraint(
                "CK_UntrackedUnits_QuantityAtLocation_NonNegative",
                "[QuantityAtLocation] >= 0"));

        builder.HasKey(unit => unit.Id);
        builder.HasIndex(unit => new { unit.CatalogItemId, unit.LocationId }).IsUnique();

        builder.Property(unit => unit.Notes).HasMaxLength(4_000);
        builder.Property(unit => unit.RowVersion).IsRowVersion();

        builder.HasOne(unit => unit.CatalogItem)
            .WithMany(item => item.UntrackedUnits)
            .HasForeignKey(unit => unit.CatalogItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(unit => unit.Location)
            .WithMany(location => location.UntrackedUnits)
            .HasForeignKey(unit => unit.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
