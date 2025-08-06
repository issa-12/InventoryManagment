using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventoryManagment.Model;
using System.Linq;

namespace InventoryManagment.Pages
{
    public class EditInventoryModel : PageModel
    {
        private readonly List<InventoryItem> _inventoryItems;

        [BindProperty]
        public InventoryItem InventoryItem { get; set; }

        public EditInventoryModel(List<InventoryItem> inventoryItems)
        {
            _inventoryItems = inventoryItems;
        }

        // GET handler to display the item in the edit form
        public IActionResult OnGet(int id)
        {
            InventoryItem = _inventoryItems.FirstOrDefault(item => item.Id == id);
            if (InventoryItem == null)
            {
                return NotFound();
            }

            return Page();
        }

        // POST handler to update the inventory item
        public IActionResult OnPost()
        {
            var itemToUpdate = _inventoryItems.FirstOrDefault(item => item.Id == InventoryItem.Id);
            if (itemToUpdate != null)
            {
                itemToUpdate.Name = InventoryItem.Name;
                itemToUpdate.Quantity = InventoryItem.Quantity;
                itemToUpdate.Category = InventoryItem.Category;
                itemToUpdate.Description = InventoryItem.Description;
                itemToUpdate.DueDate = InventoryItem.DueDate;
                itemToUpdate.Status = InventoryItem.Status;
            }

            return RedirectToPage("/Inventory"); // Redirect back to the main inventory page
        }
    }
}
