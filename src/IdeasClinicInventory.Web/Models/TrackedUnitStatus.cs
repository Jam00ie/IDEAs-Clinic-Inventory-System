using System.ComponentModel.DataAnnotations;

namespace IdeasClinicInventory.Web.Models;

/// <summary>
/// Describes the current inventory state of one individually identifiable unit.
/// </summary>
public enum TrackedUnitStatus
{
    Available,

    [Display(Name = "Available for administrators only")]
    AvailableForAdminsOnly,

    [Display(Name = "Pending checkout")]
    PendingCheckout,

    [Display(Name = "Checked out")]
    CheckedOut,

    [Display(Name = "Not returned")]
    NotReturned,

    [Display(Name = "Pending return")]
    PendingReturn,

    [Display(Name = "Using internally")]
    UsingInternally,

    Missing,

    [Display(Name = "Out of commission")]
    OutOfCommission
}
