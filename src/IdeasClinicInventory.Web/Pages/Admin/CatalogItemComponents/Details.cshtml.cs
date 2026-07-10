using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItemComponents;

public sealed class DetailsModel(ApplicationDbContext dbContext) : PageModel
{
    public CatalogItemComponent Component { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var component = await dbContext.CatalogItemComponents
            .AsNoTracking()
            .Include(component => component.CatalogItem)
            .SingleOrDefaultAsync(component => component.Id == id);
        if (component is null)
        {
            return NotFound();
        }

        Component = component;
        return Page();
    }
}
