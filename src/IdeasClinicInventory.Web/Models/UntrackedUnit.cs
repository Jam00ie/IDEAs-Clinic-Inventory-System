using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdeasClinicInventory.Web.Models;

/// <summary>
/// Represents a quantity-based stock bucket for one catalog item at one location.
/// </summary>
/// <remarks>
/// A catalog item has at most one untracked row per location. Separate physical
/// locations therefore produce separate rows and quantities.
/// </remarks>
public sealed class UntrackedUnit
{
    public int Id { get; set; }

    [Display(Name = "Catalog item")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a catalog item.")]
    public int CatalogItemId { get; set; }

    public CatalogItem CatalogItem { get; set; } = null!;

    [Display(Name = "Inventory location")]
    [Range(1, int.MaxValue, ErrorMessage = "Select an inventory location.")]
    public int LocationId { get; set; }

    public InventoryLocation Location { get; set; } = null!;

    [Display(Name = "Quantity at location")]
    [Range(0, int.MaxValue)]
    public int QuantityAtLocation { get; set; }

    [StringLength(4_000)]
    public string? Notes { get; set; }

    [Display(Name = "Created (UTC)")]
    public DateTimeOffset CreatedAtUtc { get; set; }

    [Display(Name = "Updated (UTC)")]
    public DateTimeOffset UpdatedAtUtc { get; set; }

    /// <summary>
    /// Produces labels such as "Grove Kit Unit(s) @ PSE2409A-3S2".
    /// </summary>
    [NotMapped]
    public string DisplayName
    {
        get
        {
            var catalogName = string.IsNullOrWhiteSpace(CatalogItem?.Name)
                ? "Untracked"
                : CatalogItem.Name;
            var locationCode = Location is null ? "Unknown Location" : Location.LocationCode;
            return $"{catalogName} Unit(s) @ {locationCode}";
        }
    }

    /// <summary>
    /// SQL Server changes this value on every update so simultaneous edits can be detected.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
