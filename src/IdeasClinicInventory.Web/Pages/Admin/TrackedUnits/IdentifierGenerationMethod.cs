using System.ComponentModel.DataAnnotations;

namespace IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

public enum IdentifierGenerationMethod
{
    [Display(Name = "Enter one identifier manually")]
    Manual,

    [Display(Name = "Numerical order (1, 2, 3, …)")]
    NumericalOrder,

    [Display(Name = "Alphabetical order (A, B, C, …)")]
    AlphabeticalOrder
}
