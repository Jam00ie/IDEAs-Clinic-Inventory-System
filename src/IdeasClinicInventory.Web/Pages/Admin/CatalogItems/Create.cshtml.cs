using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItems;

public sealed class CreateModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public CatalogItem Item { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Map only administrator-editable properties. Audit and concurrency fields
        // are controlled by EF/SQL Server and must not be accepted from the browser.
        var item = new CatalogItem
        {
            Name = Item.Name,
            Category = Item.Category,
            UnitOfMeasure = Item.UnitOfMeasure,
            AllowedUseType = Item.AllowedUseType,
            RestrictedToAdmins = Item.RestrictedToAdmins,
            QuantityReservedForAdmins = Item.QuantityReservedForAdmins,
            ReferenceImagePath = Item.ReferenceImagePath,
            Notes = Item.Notes
        };

        dbContext.CatalogItems.Add(item);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError("Item.Name", "A catalog item with this name already exists.");
            return Page();
        }

        TempData["StatusMessage"] = $"Created catalog item '{item.Name}'.";
        return RedirectToPage("Index");
    }
}
