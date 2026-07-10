using System.ComponentModel.DataAnnotations;

namespace IdeasClinicInventory.Web.Models;

/// <summary>
/// Defines one type of item in the inventory catalog, such as an Arduino kit or gloves.
/// </summary>
/// <remarks>
/// This record describes the catalog entry, not its physical stock. Tracked units and
/// untracked quantities will reference it when those entities are added.
/// </remarks>
public sealed class CatalogItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; set; }

    [Display(Name = "Unit of measure")]
    [StringLength(50)]
    public string? UnitOfMeasure { get; set; }

    [Display(Name = "Allowed use")]
    public AllowedUseType AllowedUseType { get; set; }

    [Display(Name = "Restricted to IDEAs Clinic administrators")]
    public bool RestrictedToAdmins { get; set; }

    [Display(Name = "Quantity reserved for IDEAs Clinic administrators")]
    [Range(0, int.MaxValue)]
    public int QuantityReservedForAdmins { get; set; }

    [Display(Name = "Reference image path")]
    [StringLength(2_048)]
    public string? ReferenceImagePath { get; set; }

    [StringLength(4_000)]
    public string? Notes { get; set; }

    [Display(Name = "Created (UTC)")]
    public DateTimeOffset CreatedAtUtc { get; set; }

    [Display(Name = "Updated (UTC)")]
    public DateTimeOffset UpdatedAtUtc { get; set; }

    /// <summary>
    /// SQL Server changes this value on every update so simultaneous edits can be detected.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    /// <summary>
    /// Individually identifiable physical units belonging to this catalog definition.
    /// </summary>
    public ICollection<TrackedUnit> TrackedUnits { get; set; } = [];

    /// <summary>
    /// Quantity-based stock buckets stored at distinct inventory locations.
    /// </summary>
    public ICollection<UntrackedUnit> UntrackedUnits { get; set; } = [];
}
