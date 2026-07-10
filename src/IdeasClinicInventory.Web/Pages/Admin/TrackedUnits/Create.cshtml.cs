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

        IReadOnlyList<string> identifiers = [];
        if (ModelState.IsValid)
        {
            try
            {
                identifiers = TrackedUnitIdentifierGenerator.Generate(
                    Input.GenerationMethod,
                    Input.Identifier,
                    Input.StartingIdentifier,
                    Input.Quantity,
                    Input.IdentifierPrefix,
                    Input.IdentifierPostfix);
            }
            catch (Exception exception) when (exception is ArgumentException or OverflowException)
            {
                var field = Input.GenerationMethod == IdentifierGenerationMethod.Manual
                    ? "Input.Identifier"
                    : "Input.StartingIdentifier";
                // ArgumentException appends a parameter name on a second line;
                // only the actionable first line belongs in the validation UI.
                var message = exception.Message.Split(Environment.NewLine, StringSplitOptions.None)[0];
                ModelState.AddModelError(field, message);
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        // Detect collisions before writing so the administrator sees every conflicting
        // identifier. The unique index still protects against simultaneous requests.
        var existingIdentifiers = await DbContext.TrackedUnits
            .Where(unit => unit.CatalogItemId == Input.CatalogItemId && identifiers.Contains(unit.Identifier))
            .Select(unit => unit.Identifier)
            .OrderBy(identifier => identifier)
            .ToListAsync();
        if (existingIdentifiers.Count > 0)
        {
            ModelState.AddModelError(
                Input.GenerationMethod == IdentifierGenerationMethod.Manual
                    ? "Input.Identifier"
                    : "Input.StartingIdentifier",
                $"These identifiers already exist for the selected catalog item: {string.Join(", ", existingIdentifiers)}.");
            await LoadOptionsAsync();
            return Page();
        }

        var units = identifiers.Select(identifier => new TrackedUnit
        {
            CatalogItemId = Input.CatalogItemId,
            Identifier = identifier,
            HomeLocationId = Input.HomeLocationId,
            Status = Input.Status,
            Notes = Input.Notes
        }).ToList();

        DbContext.TrackedUnits.AddRange(units);

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError(
                Input.GenerationMethod == IdentifierGenerationMethod.Manual
                    ? "Input.Identifier"
                    : "Input.StartingIdentifier",
                "One or more generated identifiers were created by another request. Review the batch and try again.");
            await LoadOptionsAsync();
            return Page();
        }

        TempData["StatusMessage"] = units.Count == 1
            ? $"Created tracked unit '{units[0].Identifier}'."
            : $"Created {units.Count} tracked units ({units[0].Identifier}–{units[^1].Identifier}).";

        return units.Count == 1
            ? RedirectToPage("Details", new { id = units[0].Id })
            : RedirectToPage("Index", new { catalogItemId = Input.CatalogItemId });
    }
}
