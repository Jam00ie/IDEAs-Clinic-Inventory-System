using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

public sealed class CreateModel(ApplicationDbContext dbContext) : TrackedUnitPageModel(dbContext)
{
    [BindProperty]
    public TrackedUnitInput Input { get; set; } = new();

    public async Task OnGetAsync(int? catalogItemId)
    {
        await LoadOptionsAsync();

        if (catalogItemId is not null)
        {
            Input.CatalogItemId = catalogItemId.Value;

            // A restricted catalog item starts in the matching restricted state,
            // while the administrator may still choose another state if needed.
            var restricted = await DbContext.CatalogItems
                .Where(item => item.Id == catalogItemId)
                .Select(item => (bool?)item.RestrictedToAdmins)
                .SingleOrDefaultAsync();
            if (restricted == true)
            {
                Input.Status = TrackedUnitStatus.AvailableForAdminsOnly;
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await ValidateParentIdsAsync(Input);

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var unit = new TrackedUnit
        {
            CatalogItemId = Input.CatalogItemId,
            Identifier = Input.Identifier,
            HomeLocationId = Input.HomeLocationId,
            Status = Input.Status,
            Notes = Input.Notes
        };

        DbContext.TrackedUnits.Add(unit);

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError("Input.Identifier",
                "This catalog item already has a tracked unit with that identifier.");
            await LoadOptionsAsync();
            return Page();
        }

        TempData["StatusMessage"] = $"Created tracked unit '{unit.Identifier}'.";
        return RedirectToPage("Details", new { id = unit.Id });
    }
}
