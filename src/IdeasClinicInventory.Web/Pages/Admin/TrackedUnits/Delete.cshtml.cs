using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

public sealed class DeleteModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public TrackedUnitInput Input { get; set; } = null!;

    public TrackedUnit Unit { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var unit = await LoadUnitAsync(id);
        if (unit is null)
        {
            return NotFound();
        }

        Unit = unit;
        Input = TrackedUnitInput.FromEntity(unit);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Input.Id)
        {
            return BadRequest();
        }

        var unitToDelete = await dbContext.TrackedUnits.SingleOrDefaultAsync(unit => unit.Id == id);
        if (unitToDelete is null)
        {
            TempData["StatusMessage"] = "The tracked unit was already deleted.";
            return RedirectToPage("Index");
        }

        dbContext.Entry(unitToDelete).Property(unit => unit.RowVersion).OriginalValue = Input.RowVersion;
        dbContext.TrackedUnits.Remove(unitToDelete);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentUnit = await LoadUnitAsync(id);
            if (currentUnit is null)
            {
                TempData["StatusMessage"] = "The tracked unit was already deleted.";
                return RedirectToPage("Index");
            }

            Unit = currentUnit;
            Input = TrackedUnitInput.FromEntity(currentUnit);
            ModelState.Clear();
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this unit. Review the current values before deleting it.");
            return Page();
        }
        catch (DbUpdateException)
        {
            Unit = await LoadUnitAsync(id) ?? unitToDelete;
            ModelState.AddModelError(string.Empty,
                "This tracked unit is referenced by inventory activity and cannot be deleted.");
            return Page();
        }

        TempData["StatusMessage"] = $"Deleted tracked unit '{unitToDelete.Identifier}'.";
        return RedirectToPage("Index", new { catalogItemId = unitToDelete.CatalogItemId });
    }

    private Task<TrackedUnit?> LoadUnitAsync(int id) => dbContext.TrackedUnits
        .AsNoTracking()
        .Include(unit => unit.CatalogItem)
        .Include(unit => unit.HomeLocation)
        .SingleOrDefaultAsync(unit => unit.Id == id);
}
