using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdeasClinicInventory.Web.Models;

/// <summary>
/// Defines one expected component in a single unit of its parent catalog item.
/// </summary>
/// <remarks>
/// Catalog items without component rows are treated as whole items. No synthetic
/// "Whole" component row is needed.
/// </remarks>
public sealed class CatalogItemComponent
{
    public int Id { get; set; }

    [Display(Name = "Catalog item")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a catalog item.")]
    public int CatalogItemId { get; set; }

    public CatalogItem CatalogItem { get; set; } = null!;

    [Display(Name = "Component name")]
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Expected quantity per unit")]
    [Range(1, int.MaxValue)]
    public int ExpectedQuantity { get; set; } = 1;

    [Display(Name = "Created (UTC)")]
    public DateTimeOffset CreatedAtUtc { get; set; }

    [Display(Name = "Updated (UTC)")]
    public DateTimeOffset UpdatedAtUtc { get; set; }

    [NotMapped]
    public string DisplayName => $"{Name} ({ExpectedQuantity})";

    /// <summary>
    /// SQL Server changes this value on every update so simultaneous edits can be detected.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
