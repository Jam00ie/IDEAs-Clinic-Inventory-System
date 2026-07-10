using IdeasClinicInventory.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeasClinicInventory.Web.Data.Configurations;

/// <summary>
/// Defines SQL constraints and indexes for physical inventory locations.
/// </summary>
public sealed class InventoryLocationConfiguration : IEntityTypeConfiguration<InventoryLocation>
{
    public void Configure(EntityTypeBuilder<InventoryLocation> builder)
    {
        builder.ToTable("InventoryLocations", table =>
            table.HasCheckConstraint(
                "CK_InventoryLocations_StorageUnitLevel_NonNegative",
                "[StorageUnitLevel] >= 0"));

        builder.HasKey(location => location.Id);

        // A physical location must appear only once. Codes are normalized before save,
        // so this index also treats differently cased input as the same location.
        builder.HasIndex(location => new
        {
            location.RoomCode,
            location.StorageUnitLevel,
            location.StorageUnitCode,
            location.Subunit
        })
            .IsUnique()
            // SQL Server's EF convention filters nullable columns out of unique
            // indexes. Removing that filter permits only one matching null subunit.
            .HasFilter(null);

        builder.Property(location => location.RoomCode).HasMaxLength(20).IsRequired();
        builder.Property(location => location.StorageUnitCode).HasMaxLength(20).IsRequired();
        builder.Property(location => location.Subunit).HasMaxLength(50);
        builder.Property(location => location.ReferenceImagePath).HasMaxLength(2_048);
        builder.Property(location => location.Notes).HasMaxLength(4_000);
        builder.Property(location => location.RowVersion).IsRowVersion();
    }
}
