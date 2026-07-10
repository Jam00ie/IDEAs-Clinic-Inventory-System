using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.InventoryLocations;

public sealed class CreateModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public InventoryLocation Location { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var location = new InventoryLocation
        {
            RoomCode = Location.RoomCode,
            StorageUnitCode = Location.StorageUnitCode,
            StorageUnitLevel = Location.StorageUnitLevel,
            Subunit = Location.Subunit,
            ReferenceImagePath = Location.ReferenceImagePath,
            Notes = Location.Notes
        };

        dbContext.InventoryLocations.Add(location);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError(string.Empty,
                "This room, storage unit, level, and subunit combination already exists.");
            return Page();
        }

        TempData["StatusMessage"] = $"Created inventory location '{location.LocationCode}'.";
        return RedirectToPage("Index");
    }
}
