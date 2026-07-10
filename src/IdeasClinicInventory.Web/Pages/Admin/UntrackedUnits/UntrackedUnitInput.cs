using System.ComponentModel.DataAnnotations;
using IdeasClinicInventory.Web.Models;

namespace IdeasClinicInventory.Web.Pages.Admin.UntrackedUnits;

/// <summary>
/// Contains only the untracked-stock fields administrators may submit.
/// </summary>
public sealed class UntrackedUnitInput
{
    public int Id { get; set; }

    [Display(Name = "Catalog item")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a catalog item.")]
    public int CatalogItemId { get; set; }

    [Display(Name = "Inventory location")]
    [Range(1, int.MaxValue, ErrorMessage = "Select an inventory location.")]
    public int LocationId { get; set; }

    [Display(Name = "Quantity at location")]
    [Range(0, int.MaxValue)]
    public int QuantityAtLocation { get; set; }

    [StringLength(4_000)]
    public string? Notes { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public static UntrackedUnitInput FromEntity(UntrackedUnit unit) => new()
    {
        Id = unit.Id,
        CatalogItemId = unit.CatalogItemId,
        LocationId = unit.LocationId,
        QuantityAtLocation = unit.QuantityAtLocation,
        Notes = unit.Notes,
        RowVersion = unit.RowVersion
    };
}
