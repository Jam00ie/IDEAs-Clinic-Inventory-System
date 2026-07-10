using IdeasClinicInventory.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

public sealed class EditModel(ApplicationDbContext dbContext) : TrackedUnitPageModel(dbContext)
{
    [BindProperty]
    public TrackedUnitInput Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var unit = await DbContext.TrackedUnits.AsNoTracking().SingleOrDefaultAsync(unit => unit.Id == id);
        if (unit is null)
        {
            return NotFound();
        }

        Input = TrackedUnitInput.FromEntity(unit);
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

        var unitToUpdate = await DbContext.TrackedUnits.SingleOrDefaultAsync(unit => unit.Id == id);
        if (unitToUpdate is null)
        {
            return NotFound();
        }

        DbContext.Entry(unitToUpdate).Property(unit => unit.RowVersion).OriginalValue = Input.RowVersion;
        unitToUpdate.CatalogItemId = Input.CatalogItemId;
        unitToUpdate.Identifier = Input.Identifier;
        unitToUpdate.HomeLocationId = Input.HomeLocationId;
        unitToUpdate.Status = Input.Status;
        unitToUpdate.Notes = Input.Notes;

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentUnit = await DbContext.TrackedUnits.AsNoTracking().SingleOrDefaultAsync(unit => unit.Id == id);
            if (currentUnit is null)
            {
                return NotFound();
            }

            Input.RowVersion = currentUnit.RowVersion;
            ModelState.Remove("Input.RowVersion");
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this unit. Review your values and save again to overwrite the newer version.");
            await LoadOptionsAsync();
            return Page();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError("Input.Identifier",
                "This catalog item already has a tracked unit with that identifier.");
            await LoadOptionsAsync();
            return Page();
        }

        TempData["StatusMessage"] = $"Updated tracked unit '{unitToUpdate.Identifier}'.";
        return RedirectToPage("Details", new { id });
    }
}
