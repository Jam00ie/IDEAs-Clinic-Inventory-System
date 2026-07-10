using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.InventoryLocations;

public sealed class DeleteModel(ApplicationDbContext dbContext) : PageModel
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

        var locationToDelete = await dbContext.InventoryLocations
            .SingleOrDefaultAsync(location => location.Id == id);
        if (locationToDelete is null)
        {
            TempData["StatusMessage"] = "The inventory location was already deleted.";
            return RedirectToPage("Index");
        }

        dbContext.Entry(locationToDelete).Property(location => location.RowVersion).OriginalValue = Location.RowVersion;
        dbContext.InventoryLocations.Remove(locationToDelete);

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
                TempData["StatusMessage"] = "The inventory location was already deleted.";
                return RedirectToPage("Index");
            }

            Location = currentLocation;
            ModelState.Clear();
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this location. Review the current values before deleting it.");
            return Page();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty,
                "This location is referenced by inventory records and cannot be deleted.");
            Location = locationToDelete;
            return Page();
        }

        TempData["StatusMessage"] = $"Deleted inventory location '{locationToDelete.LocationCode}'.";
        return RedirectToPage("Index");
    }
}
