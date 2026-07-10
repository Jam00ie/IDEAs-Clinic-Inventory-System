using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

public sealed class DetailsModel(ApplicationDbContext dbContext) : PageModel
{
    public TrackedUnit Unit { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var unit = await dbContext.TrackedUnits
            .AsNoTracking()
            .Include(unit => unit.CatalogItem)
            .Include(unit => unit.HomeLocation)
            .SingleOrDefaultAsync(unit => unit.Id == id);
        if (unit is null)
        {
            return NotFound();
        }

        Unit = unit;
        return Page();
    }
}
