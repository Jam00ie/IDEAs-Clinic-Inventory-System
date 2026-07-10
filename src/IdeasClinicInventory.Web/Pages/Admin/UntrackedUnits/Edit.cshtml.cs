using IdeasClinicInventory.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.UntrackedUnits;

public sealed class EditModel(ApplicationDbContext dbContext) : UntrackedUnitPageModel(dbContext)
{
    [BindProperty]
    public UntrackedUnitInput Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var unit = await DbContext.UntrackedUnits.AsNoTracking().SingleOrDefaultAsync(unit => unit.Id == id);
        if (unit is null)
        {
            return NotFound();
        }

        Input = UntrackedUnitInput.FromEntity(unit);
        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Input.Id)
        {
            return BadRequest();
        }

        await ValidateParentIdsAsync(Input);
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var unitToUpdate = await DbContext.UntrackedUnits.SingleOrDefaultAsync(unit => unit.Id == id);
        if (unitToUpdate is null)
        {
            return NotFound();
        }

        DbContext.Entry(unitToUpdate).Property(unit => unit.RowVersion).OriginalValue = Input.RowVersion;
        unitToUpdate.CatalogItemId = Input.CatalogItemId;
        unitToUpdate.LocationId = Input.LocationId;
        unitToUpdate.QuantityAtLocation = Input.QuantityAtLocation;
        unitToUpdate.Notes = Input.Notes;

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentUnit = await DbContext.UntrackedUnits.AsNoTracking().SingleOrDefaultAsync(unit => unit.Id == id);
            if (currentUnit is null)
            {
                return NotFound();
            }

            Input.RowVersion = currentUnit.RowVersion;
            ModelState.Remove("Input.RowVersion");
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this entry. Review your values and save again to overwrite the newer version.");
            await LoadOptionsAsync();
            return Page();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError(string.Empty,
                "This catalog item already has an untracked quantity at the selected location.");
            await LoadOptionsAsync();
            return Page();
        }

        TempData["StatusMessage"] = "Updated the untracked quantity entry.";
        return RedirectToPage("Details", new { id });
    }
}
