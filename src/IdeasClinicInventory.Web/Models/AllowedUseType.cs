using System.ComponentModel.DataAnnotations;

namespace IdeasClinicInventory.Web.Models;

/// <summary>
/// Describes how a catalog item may leave IDEAs Clinic inventory.
/// </summary>
public enum AllowedUseType
{
    None,
    Checkout,
    Dispense,

    [Display(Name = "Checkout and dispense")]
    CheckoutAndDispense
}
