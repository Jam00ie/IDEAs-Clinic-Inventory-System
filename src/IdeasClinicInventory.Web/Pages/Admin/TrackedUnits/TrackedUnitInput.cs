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
public sealed class TrackedUnitInput : IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "Catalog item")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a catalog item.")]
    public int CatalogItemId { get; set; }

    [StringLength(100)]
    public string? Identifier { get; set; }

    [Display(Name = "Identifier generation method")]
    public IdentifierGenerationMethod GenerationMethod { get; set; }

    [Display(Name = "Starting identifier")]
    [StringLength(100)]
    public string? StartingIdentifier { get; set; }

    [Display(Name = "Identifier prefix")]
    [StringLength(50)]
    public string? IdentifierPrefix { get; set; }

    [Display(Name = "Identifier postfix")]
    [StringLength(50)]
    public string? IdentifierPostfix { get; set; }

    [Display(Name = "Number of units")]
    [Range(1, TrackedUnitIdentifierGenerator.MaximumBatchSize)]
    public int Quantity { get; set; } = 1;

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (GenerationMethod == IdentifierGenerationMethod.Manual && string.IsNullOrWhiteSpace(Identifier))
        {
            yield return new ValidationResult("Enter an identifier.", [nameof(Identifier)]);
        }

        if (GenerationMethod != IdentifierGenerationMethod.Manual && string.IsNullOrWhiteSpace(StartingIdentifier))
        {
            yield return new ValidationResult("Enter a starting identifier.", [nameof(StartingIdentifier)]);
        }
    }
}
