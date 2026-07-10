using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.UntrackedUnits;

public sealed class IndexModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? CatalogItemId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? LocationId { get; set; }

    public IReadOnlyList<UntrackedUnit> UntrackedUnits { get; private set; } = [];

    public IReadOnlyList<SelectListItem> CatalogItemOptions { get; private set; } = [];

    public IReadOnlyList<SelectListItem> LocationOptions { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var query = dbContext.UntrackedUnits
            .AsNoTracking()
            .Include(unit => unit.CatalogItem)
            .Include(unit => unit.Location)
            .AsQueryable();

        if (CatalogItemId is not null)
        {
            query = query.Where(unit => unit.CatalogItemId == CatalogItemId);
        }

        if (LocationId is not null)
        {
            query = query.Where(unit => unit.LocationId == LocationId);
        }

        UntrackedUnits = await query
            .OrderBy(unit => unit.CatalogItem.Name)
            .ThenBy(unit => unit.Location.RoomCode)
            .ThenBy(unit => unit.Location.StorageUnitLevel)
            .ThenBy(unit => unit.Location.StorageUnitCode)
            .ToListAsync();

        CatalogItemOptions = await dbContext.CatalogItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new SelectListItem(item.Name, item.Id.ToString()))
            .ToListAsync();

        var locations = await dbContext.InventoryLocations.AsNoTracking().ToListAsync();
        LocationOptions = locations
            .OrderBy(location => location.LocationCode)
            .Select(location => new SelectListItem(location.LocationCode, location.Id.ToString()))
            .ToList();
    }
}
