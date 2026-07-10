using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItemComponents;

public sealed class DeleteModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public CatalogItemComponentInput Input { get; set; } = null!;

    public CatalogItemComponent Component { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var component = await LoadComponentAsync(id);
        if (component is null)
        {
            return NotFound();
        }

        Component = component;
        Input = CatalogItemComponentInput.FromEntity(component);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Input.Id)
        {
            return BadRequest();
        }

        var componentToDelete = await dbContext.CatalogItemComponents
            .SingleOrDefaultAsync(component => component.Id == id);
        if (componentToDelete is null)
        {
            TempData["StatusMessage"] = "The component was already deleted.";
            return RedirectToPage("Index");
        }

        dbContext.Entry(componentToDelete).Property(component => component.RowVersion).OriginalValue = Input.RowVersion;
        dbContext.CatalogItemComponents.Remove(componentToDelete);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentComponent = await LoadComponentAsync(id);
            if (currentComponent is null)
            {
                TempData["StatusMessage"] = "The component was already deleted.";
                return RedirectToPage("Index");
            }

            Component = currentComponent;
            Input = CatalogItemComponentInput.FromEntity(currentComponent);
            ModelState.Clear();
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this component. Review the current values before deleting it.");
            return Page();
        }

        TempData["StatusMessage"] = $"Deleted component '{componentToDelete.Name}'.";
        return RedirectToPage("Index", new { catalogItemId = componentToDelete.CatalogItemId });
    }

    private Task<CatalogItemComponent?> LoadComponentAsync(int id) => dbContext.CatalogItemComponents
        .AsNoTracking()
        .Include(component => component.CatalogItem)
        .SingleOrDefaultAsync(component => component.Id == id);
}
