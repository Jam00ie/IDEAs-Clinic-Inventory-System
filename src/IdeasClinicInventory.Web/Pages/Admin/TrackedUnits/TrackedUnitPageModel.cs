using IdeasClinicInventory.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

/// <summary>
/// Shares select-list loading between tracked-unit create and edit pages.
/// </summary>
public abstract class TrackedUnitPageModel(ApplicationDbContext dbContext) : PageModel
{
    protected ApplicationDbContext DbContext { get; } = dbContext;

    public IReadOnlyList<SelectListItem> CatalogItemOptions { get; private set; } = [];

    public IReadOnlyList<SelectListItem> HomeLocationOptions { get; private set; } = [];

    protected async Task LoadOptionsAsync()
    {
        CatalogItemOptions = await DbContext.CatalogItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new SelectListItem(item.Name, item.Id.ToString()))
            .ToListAsync();

        // LocationCode is intentionally derived in C#, so materialize the small
        // location table before constructing its display labels.
        var locations = await DbContext.InventoryLocations
            .AsNoTracking()
            .OrderBy(location => location.RoomCode)
            .ThenBy(location => location.StorageUnitLevel)
            .ThenBy(location => location.StorageUnitCode)
            .ThenBy(location => location.Subunit)
            .ToListAsync();

        HomeLocationOptions = locations
            .Select(location => new SelectListItem(location.LocationCode, location.Id.ToString()))
            .ToList();
    }

    protected async Task<bool> ValidateParentIdsAsync(TrackedUnitInput input)
    {
        if (!await DbContext.CatalogItems.AnyAsync(item => item.Id == input.CatalogItemId))
        {
            ModelState.AddModelError("Input.CatalogItemId", "The selected catalog item no longer exists.");
        }

        if (!await DbContext.InventoryLocations.AnyAsync(location => location.Id == input.HomeLocationId))
        {
            ModelState.AddModelError("Input.HomeLocationId", "The selected home location no longer exists.");
        }

        return ModelState.IsValid;
    }
}
