using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.InventoryLocations;

public sealed class DetailsModel(ApplicationDbContext dbContext) : PageModel
{
    public InventoryLocation Location { get; private set; } = null!;

    public IReadOnlyList<TrackedUnit> StoredTrackedUnits { get; private set; } = [];

    public IReadOnlyList<UntrackedUnit> StoredUntrackedUnits { get; private set; } = [];

    public int StoredUntrackedQuantity => StoredUntrackedUnits.Sum(unit => unit.QuantityAtLocation);

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var location = await dbContext.InventoryLocations.AsNoTracking()
            .SingleOrDefaultAsync(location => location.Id == id);
        if (location is null)
        {
            return NotFound();
        }

        Location = location;
        StoredTrackedUnits = await dbContext.TrackedUnits
            .AsNoTracking()
            .Include(unit => unit.CatalogItem)
            .Where(unit => unit.HomeLocationId == id)
            .OrderBy(unit => unit.CatalogItem.Name)
            .ThenBy(unit => unit.Identifier)
            .ToListAsync();
        StoredUntrackedUnits = await dbContext.UntrackedUnits
            .AsNoTracking()
            .Include(unit => unit.CatalogItem)
            .Where(unit => unit.LocationId == id)
            .OrderBy(unit => unit.CatalogItem.Name)
            .ToListAsync();
        return Page();
    }
}
