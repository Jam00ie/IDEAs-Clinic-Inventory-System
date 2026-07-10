using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.InventoryLocations;

public sealed class IndexModel(ApplicationDbContext dbContext) : PageModel
{
    public IReadOnlyList<InventoryLocation> InventoryLocations { get; private set; } = [];

    public async Task OnGetAsync()
    {
        InventoryLocations = await dbContext.InventoryLocations
            .AsNoTracking()
            .OrderBy(location => location.RoomCode)
            .ThenBy(location => location.StorageUnitLevel)
            .ThenBy(location => location.StorageUnitCode)
            .ThenBy(location => location.Subunit)
            .ToListAsync();
    }
}
