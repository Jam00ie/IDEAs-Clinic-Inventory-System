using IdeasClinicInventory.Web.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItemComponents;

public abstract class CatalogItemComponentPageModel(ApplicationDbContext dbContext) : PageModel
{
    protected ApplicationDbContext DbContext { get; } = dbContext;

    public IReadOnlyList<SelectListItem> CatalogItemOptions { get; private set; } = [];

    protected async Task LoadOptionsAsync()
    {
        CatalogItemOptions = await DbContext.CatalogItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new SelectListItem(item.Name, item.Id.ToString()))
            .ToListAsync();
    }

    protected async Task ValidateCatalogItemAsync(CatalogItemComponentInput input)
    {
        if (!await DbContext.CatalogItems.AnyAsync(item => item.Id == input.CatalogItemId))
        {
            ModelState.AddModelError("Input.CatalogItemId", "The selected catalog item no longer exists.");
        }
    }
}
