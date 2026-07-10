using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItems;

public sealed class EditModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty]
    public CatalogItem Item { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var item = await dbContext.CatalogItems.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        Item = item;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Item.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var itemToUpdate = await dbContext.CatalogItems.SingleOrDefaultAsync(item => item.Id == id);
        if (itemToUpdate is null)
        {
            return NotFound();
        }

        // RowVersion came from the GET response. Using it as OriginalValue makes
        // EF reject this update if another administrator saved first.
        dbContext.Entry(itemToUpdate).Property(item => item.RowVersion).OriginalValue = Item.RowVersion;

        itemToUpdate.Name = Item.Name;
        itemToUpdate.Category = Item.Category;
        itemToUpdate.UnitOfMeasure = Item.UnitOfMeasure;
        itemToUpdate.AllowedUseType = Item.AllowedUseType;
        itemToUpdate.RestrictedToAdmins = Item.RestrictedToAdmins;
        itemToUpdate.QuantityReservedForAdmins = Item.QuantityReservedForAdmins;
        itemToUpdate.ReferenceImagePath = Item.ReferenceImagePath;
        itemToUpdate.Notes = Item.Notes;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentItem = await dbContext.CatalogItems.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
            if (currentItem is null)
            {
                return NotFound();
            }

            Item.RowVersion = currentItem.RowVersion;
            ModelState.Remove("Item.RowVersion");
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this item. Review your values and save again to overwrite the newer version.");
            return Page();
        }
        catch (DbUpdateException exception) when (exception.IsUniqueConstraintViolation())
        {
            ModelState.AddModelError("Item.Name", "A catalog item with this name already exists.");
            return Page();
        }

        TempData["StatusMessage"] = $"Updated catalog item '{itemToUpdate.Name}'.";
        return RedirectToPage("Index");
    }
}
