using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItems;

public sealed class IndexModel(ApplicationDbContext dbContext) : PageModel
{
    public IReadOnlyList<CatalogItem> CatalogItems { get; private set; } = [];

    public IReadOnlyDictionary<int, int> TrackedUnitCounts { get; private set; }
        = new Dictionary<int, int>();

    public async Task OnGetAsync()
    {
        // List pages are read-only, so no-tracking queries avoid unnecessary
        // change-tracking overhead while preserving deterministic ordering.
        CatalogItems = await dbContext.CatalogItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .ToListAsync();

        TrackedUnitCounts = await dbContext.TrackedUnits
            .AsNoTracking()
            .GroupBy(unit => unit.CatalogItemId)
            .ToDictionaryAsync(group => group.Key, group => group.Count());
    }

    public int GetTrackedUnitCount(int catalogItemId) =>
        TrackedUnitCounts.GetValueOrDefault(catalogItemId);
}
