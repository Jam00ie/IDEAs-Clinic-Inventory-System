using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItems;

public sealed class DetailsModel(ApplicationDbContext dbContext) : PageModel
{
    public CatalogItem Item { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var item = await dbContext.CatalogItems.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        Item = item;
        return Page();
    }
}
