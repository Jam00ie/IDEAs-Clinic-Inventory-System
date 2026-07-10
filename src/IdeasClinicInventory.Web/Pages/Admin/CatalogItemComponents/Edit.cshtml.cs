using IdeasClinicInventory.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItemComponents;

public sealed class EditModel(ApplicationDbContext dbContext) : CatalogItemComponentPageModel(dbContext)
{
    [BindProperty]
    public CatalogItemComponentInput Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var component = await DbContext.CatalogItemComponents.AsNoTracking()
            .SingleOrDefaultAsync(component => component.Id == id);
        if (component is null)
        {
            return NotFound();
        }

        Input = CatalogItemComponentInput.FromEntity(component);
        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Input.Id)
        {
            return BadRequest();
        }

        await ValidateCatalogItemAsync(Input);
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var componentToUpdate = await DbContext.CatalogItemComponents
            .SingleOrDefaultAsync(component => component.Id == id);
        if (componentToUpdate is null)
        {
            return NotFound();
        }

        DbContext.Entry(componentToUpdate).Property(component => component.RowVersion).OriginalValue = Input.RowVersion;
        componentToUpdate.CatalogItemId = Input.CatalogItemId;
        componentToUpdate.Name = Input.Name;
        componentToUpdate.ExpectedQuantity = Input.ExpectedQuantity;

        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentComponent = await DbContext.CatalogItemComponents.AsNoTracking()
                .SingleOrDefaultAsync(component => component.Id == id);
            if (currentComponent is null)
            {
                return NotFound();
            }

            Input.RowVersion = currentComponent.RowVersion;
            ModelState.Remove("Input.RowVersion");
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this component. Review your values and save again to overwrite the newer version.");
            await LoadOptionsAsync();
            return Page();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError("Input.Name",
                "The selected catalog item already has a component with this name.");
            await LoadOptionsAsync();
            return Page();
        }

        TempData["StatusMessage"] = $"Updated component '{componentToUpdate.Name}'.";
        return RedirectToPage("Details", new { id });
    }
}
