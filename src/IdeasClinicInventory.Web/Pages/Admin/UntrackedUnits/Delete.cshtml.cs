using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.UntrackedUnits;

public sealed class DeleteModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public UntrackedUnitInput Input { get; set; } = null!;

    public UntrackedUnit Unit { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var unit = await LoadUnitAsync(id);
        if (unit is null)
        {
            return NotFound();
        }

        Unit = unit;
        Input = UntrackedUnitInput.FromEntity(unit);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Input.Id)
        {
            return BadRequest();
        }

        var unitToDelete = await dbContext.UntrackedUnits.SingleOrDefaultAsync(unit => unit.Id == id);
        if (unitToDelete is null)
        {
            TempData["StatusMessage"] = "The untracked quantity entry was already deleted.";
            return RedirectToPage("Index");
        }

        dbContext.Entry(unitToDelete).Property(unit => unit.RowVersion).OriginalValue = Input.RowVersion;
        dbContext.UntrackedUnits.Remove(unitToDelete);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentUnit = await LoadUnitAsync(id);
            if (currentUnit is null)
            {
                TempData["StatusMessage"] = "The untracked quantity entry was already deleted.";
                return RedirectToPage("Index");
            }

            Unit = currentUnit;
            Input = UntrackedUnitInput.FromEntity(currentUnit);
            ModelState.Clear();
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this entry. Review the current values before deleting it.");
            return Page();
        }

        TempData["StatusMessage"] = "Deleted the untracked quantity entry.";
        return RedirectToPage("Index", new { catalogItemId = unitToDelete.CatalogItemId });
    }

    private Task<UntrackedUnit?> LoadUnitAsync(int id) => dbContext.UntrackedUnits
        .AsNoTracking()
        .Include(unit => unit.CatalogItem)
        .Include(unit => unit.Location)
        .SingleOrDefaultAsync(unit => unit.Id == id);
}
