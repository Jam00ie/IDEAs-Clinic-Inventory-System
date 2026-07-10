using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItems;

public sealed class IndexModel(ApplicationDbContext dbContext) : PageModel
{
    public IReadOnlyList<CatalogItem> CatalogItems { get; private set; } = [];

    public async Task OnGetAsync()
    {
        // List pages are read-only, so no-tracking queries avoid unnecessary
        // change-tracking overhead while preserving deterministic ordering.
        CatalogItems = await dbContext.CatalogItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .ToListAsync();
    }
}
