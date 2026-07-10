using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.InventoryLocations;

public sealed class EditModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public InventoryLocation Location { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var location = await dbContext.InventoryLocations.AsNoTracking()
            .SingleOrDefaultAsync(location => location.Id == id);
        if (location is null)
        {
            return NotFound();
        }

        Location = location;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Location.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var locationToUpdate = await dbContext.InventoryLocations
            .SingleOrDefaultAsync(location => location.Id == id);
        if (locationToUpdate is null)
        {
            return NotFound();
        }

        dbContext.Entry(locationToUpdate).Property(location => location.RowVersion).OriginalValue = Location.RowVersion;

        locationToUpdate.RoomCode = Location.RoomCode;
        locationToUpdate.StorageUnitCode = Location.StorageUnitCode;
        locationToUpdate.StorageUnitLevel = Location.StorageUnitLevel;
        locationToUpdate.Subunit = Location.Subunit;
        locationToUpdate.ReferenceImagePath = Location.ReferenceImagePath;
        locationToUpdate.Notes = Location.Notes;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentLocation = await dbContext.InventoryLocations.AsNoTracking()
                .SingleOrDefaultAsync(location => location.Id == id);
            if (currentLocation is null)
            {
                return NotFound();
            }

            Location.RowVersion = currentLocation.RowVersion;
            ModelState.Remove("Location.RowVersion");
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this location. Review your values and save again to overwrite the newer version.");
            return Page();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError(string.Empty,
                "This room, storage unit, level, and subunit combination already exists.");
            return Page();
        }

        TempData["StatusMessage"] = $"Updated inventory location '{locationToUpdate.LocationCode}'.";
        return RedirectToPage("Index");
    }
}
