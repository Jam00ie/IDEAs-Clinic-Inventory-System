using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdeasClinicInventory.Web.Models;

/// <summary>
/// Represents one individually identifiable physical unit of a catalog item.
/// </summary>
public sealed class TrackedUnit
{
    public int Id { get; set; }

    [Display(Name = "Catalog item")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a catalog item.")]
    public int CatalogItemId { get; set; }

    public CatalogItem CatalogItem { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Identifier { get; set; } = string.Empty;

    [Display(Name = "Home location")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a home location.")]
    public int HomeLocationId { get; set; }

    public InventoryLocation HomeLocation { get; set; } = null!;

    public TrackedUnitStatus Status { get; set; } = TrackedUnitStatus.Available;

    [StringLength(4_000)]
    public string? Notes { get; set; }

    [Display(Name = "Created (UTC)")]
    public DateTimeOffset CreatedAtUtc { get; set; }

    [Display(Name = "Updated (UTC)")]
    public DateTimeOffset UpdatedAtUtc { get; set; }

    /// <summary>
    /// Combines the parent catalog name and this unit's identifier for display.
    /// </summary>
    [NotMapped]
    public string DisplayName => string.IsNullOrWhiteSpace(CatalogItem?.Name)
        ? Identifier
        : $"{CatalogItem.Name} [{Identifier}]";

    /// <summary>
    /// SQL Server changes this value on every update so simultaneous edits can be detected.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
