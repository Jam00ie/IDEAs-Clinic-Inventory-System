using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.InventoryLocations;

public sealed class DetailsModel(ApplicationDbContext dbContext) : PageModel
{
    public InventoryLocation Location { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var location = await dbContext.InventoryLocations.AsNoTracking()
            .SingleOrDefaultAsync(location => location.Id == id);
        if (location is null)
        {
            return NotFound();
        }

        Location = location;
        return Page();
    }
}
