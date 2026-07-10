using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItemComponents;

public sealed class CreateModel(ApplicationDbContext dbContext) : CatalogItemComponentPageModel(dbContext)
{
    [BindProperty]
    public CatalogItemComponentInput Input { get; set; } = new();

    public async Task OnGetAsync(int? catalogItemId)
    {
        Input.CatalogItemId = catalogItemId ?? 0;
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await ValidateCatalogItemAsync(Input);
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var component = new CatalogItemComponent
        {
            CatalogItemId = Input.CatalogItemId,
            Name = Input.Name,
            ExpectedQuantity = Input.ExpectedQuantity
        };

        DbContext.CatalogItemComponents.Add(component);

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError("Input.Name",
                "The selected catalog item already has a component with this name.");
            await LoadOptionsAsync();
            return Page();
        }

        TempData["StatusMessage"] = $"Created component '{component.Name}'.";
        return RedirectToPage("Details", new { id = component.Id });
    }
}
