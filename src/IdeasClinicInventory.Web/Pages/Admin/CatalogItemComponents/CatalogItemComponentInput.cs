using System.ComponentModel.DataAnnotations;
using IdeasClinicInventory.Web.Models;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItemComponents;

/// <summary>
/// Contains the component fields administrators may submit.
/// </summary>
public sealed class CatalogItemComponentInput
{
    public int Id { get; set; }

    [Display(Name = "Catalog item")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a catalog item.")]
    public int CatalogItemId { get; set; }

    [Display(Name = "Component name")]
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Expected quantity per unit")]
    [Range(1, int.MaxValue)]
    public int ExpectedQuantity { get; set; } = 1;

    public byte[] RowVersion { get; set; } = [];

    public static CatalogItemComponentInput FromEntity(CatalogItemComponent component) => new()
    {
        Id = component.Id,
        CatalogItemId = component.CatalogItemId,
        Name = component.Name,
        ExpectedQuantity = component.ExpectedQuantity,
        RowVersion = component.RowVersion
    };
}
