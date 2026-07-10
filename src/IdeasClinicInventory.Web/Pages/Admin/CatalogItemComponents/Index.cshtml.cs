using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItemComponents;

public sealed class IndexModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? CatalogItemId { get; set; }

    public IReadOnlyList<CatalogItemComponent> Components { get; private set; } = [];

    public IReadOnlyList<SelectListItem> CatalogItemOptions { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var query = dbContext.CatalogItemComponents
            .AsNoTracking()
            .Include(component => component.CatalogItem)
            .AsQueryable();

        if (CatalogItemId is not null)
        {
            query = query.Where(component => component.CatalogItemId == CatalogItemId);
        }

        Components = await query
            .OrderBy(component => component.CatalogItem.Name)
            .ThenBy(component => component.Name)
            .ToListAsync();

        CatalogItemOptions = await dbContext.CatalogItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new SelectListItem(item.Name, item.Id.ToString()))
            .ToListAsync();
    }
}
