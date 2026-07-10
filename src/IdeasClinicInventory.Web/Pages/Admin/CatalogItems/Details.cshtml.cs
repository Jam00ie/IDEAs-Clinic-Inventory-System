using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItems;

public sealed class DetailsModel(ApplicationDbContext dbContext) : PageModel
{
    public CatalogItem Item { get; private set; } = null!;

    public IReadOnlyList<TrackedUnit> TrackedUnits { get; private set; } = [];

    public IReadOnlyList<UntrackedUnit> UntrackedUnits { get; private set; } = [];

    public int UntrackedQuantity => UntrackedUnits.Sum(unit => unit.QuantityAtLocation);

    public int TotalCountedQuantity => TrackedUnits.Count + UntrackedQuantity;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var item = await dbContext.CatalogItems.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        Item = item;
        TrackedUnits = await dbContext.TrackedUnits
            .AsNoTracking()
            .Include(unit => unit.HomeLocation)
            .Where(unit => unit.CatalogItemId == id)
            .OrderBy(unit => unit.Identifier)
            .ToListAsync();
        UntrackedUnits = await dbContext.UntrackedUnits
            .AsNoTracking()
            .Include(unit => unit.Location)
            .Where(unit => unit.CatalogItemId == id)
            .OrderBy(unit => unit.Location.RoomCode)
            .ThenBy(unit => unit.Location.StorageUnitLevel)
            .ThenBy(unit => unit.Location.StorageUnitCode)
            .ToListAsync();
        return Page();
    }
}
