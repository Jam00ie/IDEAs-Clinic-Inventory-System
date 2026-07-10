using IdeasClinicInventory.Web.Data;
using IdeasClinicInventory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Pages.Admin.CatalogItems;

public sealed class DeleteModel(ApplicationDbContext dbContext) : PageModel
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

        var itemToDelete = await dbContext.CatalogItems.SingleOrDefaultAsync(item => item.Id == id);
        if (itemToDelete is null)
        {
            TempData["StatusMessage"] = "The catalog item was already deleted.";
            return RedirectToPage("Index");
        }

        // Prevent a stale confirmation page from deleting a record that another
        // administrator edited after the confirmation page was loaded.
        dbContext.Entry(itemToDelete).Property(item => item.RowVersion).OriginalValue = Item.RowVersion;
        dbContext.CatalogItems.Remove(itemToDelete);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var currentItem = await dbContext.CatalogItems.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
            if (currentItem is null)
            {
                TempData["StatusMessage"] = "The catalog item was already deleted.";
                return RedirectToPage("Index");
            }

            Item = currentItem;
            ModelState.Clear();
            ModelState.AddModelError(string.Empty,
                "Another administrator changed this item. Review the current values before deleting it.");
            return Page();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty,
                "This catalog item is referenced by inventory records and cannot be deleted.");
            Item = itemToDelete;
            return Page();
        }

        TempData["StatusMessage"] = $"Deleted catalog item '{itemToDelete.Name}'.";
        return RedirectToPage("Index");
    }
}
