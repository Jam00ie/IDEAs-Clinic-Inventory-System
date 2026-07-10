using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.UntrackedUnits;

public sealed class CreateModel(ApplicationDbContext dbContext) : UntrackedUnitPageModel(dbContext)
{
    [BindProperty]
    public UntrackedUnitInput Input { get; set; } = new();

    public async Task OnGetAsync(int? catalogItemId, int? locationId)
    {
        Input.CatalogItemId = catalogItemId ?? 0;
        Input.LocationId = locationId ?? 0;
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await ValidateParentIdsAsync(Input);

        if (ModelState.IsValid && await DbContext.UntrackedUnits.AnyAsync(unit =>
                unit.CatalogItemId == Input.CatalogItemId && unit.LocationId == Input.LocationId))
        {
            ModelState.AddModelError(string.Empty,
                "This catalog item already has an untracked quantity at the selected location. Edit the existing entry instead.");
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var unit = new UntrackedUnit
        {
            CatalogItemId = Input.CatalogItemId,
            LocationId = Input.LocationId,
            QuantityAtLocation = Input.QuantityAtLocation,
            Notes = Input.Notes
        };

        DbContext.UntrackedUnits.Add(unit);

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError(string.Empty,
                "Another request created this catalog item/location entry. Edit the existing entry instead.");
            await LoadOptionsAsync();
            return Page();
        }

        TempData["StatusMessage"] = "Created the untracked quantity entry.";
        return RedirectToPage("Details", new { id = unit.Id });
    }
}
