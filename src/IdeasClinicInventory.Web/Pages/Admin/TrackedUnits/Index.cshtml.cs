using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

public sealed class IndexModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? CatalogItemId { get; set; }

    public IReadOnlyList<TrackedUnit> TrackedUnits { get; private set; } = [];

    public IReadOnlyList<SelectListItem> CatalogItemOptions { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var query = dbContext.TrackedUnits
            .AsNoTracking()
            .Include(unit => unit.CatalogItem)
            .Include(unit => unit.HomeLocation)
            .AsQueryable();

        if (CatalogItemId is not null)
        {
            query = query.Where(unit => unit.CatalogItemId == CatalogItemId);
        }

        TrackedUnits = await query
            .OrderBy(unit => unit.CatalogItem.Name)
            .ThenBy(unit => unit.Identifier)
            .ToListAsync();

        CatalogItemOptions = await dbContext.CatalogItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new SelectListItem(item.Name, item.Id.ToString()))
            .ToListAsync();
    }
}
