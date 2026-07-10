using System.ComponentModel.DataAnnotations;
using IdeasClinicInventory.Web.Models;

namespace IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

/// <summary>
/// Defines the fields administrators may submit for a tracked unit.
/// </summary>
/// <remarks>
/// A dedicated input model prevents navigation, audit, and database-generated
/// properties on TrackedUnit from being over-posted by a browser request.
/// </remarks>
public sealed class TrackedUnitInput
{
    public int Id { get; set; }

    [Display(Name = "Catalog item")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a catalog item.")]
    public int CatalogItemId { get; set; }

    [Required]
    [StringLength(100)]
    public string Identifier { get; set; } = string.Empty;

    [Display(Name = "Home location")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a home location.")]
    public int HomeLocationId { get; set; }

    public TrackedUnitStatus Status { get; set; } = TrackedUnitStatus.Available;

    [StringLength(4_000)]
    public string? Notes { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public static TrackedUnitInput FromEntity(TrackedUnit unit) => new()
    {
        Id = unit.Id,
        CatalogItemId = unit.CatalogItemId,
        Identifier = unit.Identifier,
        HomeLocationId = unit.HomeLocationId,
        Status = unit.Status,
        Notes = unit.Notes,
        RowVersion = unit.RowVersion
    };
}
