using IdeasClinicInventory.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.UntrackedUnits;

/// <summary>
/// Shares parent option and validation logic between create and edit pages.
/// </summary>
public abstract class UntrackedUnitPageModel(ApplicationDbContext dbContext) : PageModel
{
    protected ApplicationDbContext DbContext { get; } = dbContext;

    public IReadOnlyList<SelectListItem> CatalogItemOptions { get; private set; } = [];

    public IReadOnlyList<SelectListItem> LocationOptions { get; private set; } = [];

    protected async Task LoadOptionsAsync()
    {
        CatalogItemOptions = await DbContext.CatalogItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new SelectListItem(item.Name, item.Id.ToString()))
            .ToListAsync();

        var locations = await DbContext.InventoryLocations
            .AsNoTracking()
            .OrderBy(location => location.RoomCode)
            .ThenBy(location => location.StorageUnitLevel)
            .ThenBy(location => location.StorageUnitCode)
            .ThenBy(location => location.Subunit)
            .ToListAsync();

        LocationOptions = locations
            .Select(location => new SelectListItem(location.LocationCode, location.Id.ToString()))
            .ToList();
    }

    protected async Task ValidateParentIdsAsync(UntrackedUnitInput input)
    {
        if (!await DbContext.CatalogItems.AnyAsync(item => item.Id == input.CatalogItemId))
        {
            ModelState.AddModelError("Input.CatalogItemId", "The selected catalog item no longer exists.");
        }

        if (!await DbContext.InventoryLocations.AnyAsync(location => location.Id == input.LocationId))
        {
            ModelState.AddModelError("Input.LocationId", "The selected inventory location no longer exists.");
        }
    }
}
