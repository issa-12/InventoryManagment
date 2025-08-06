using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventoryManagment.Model;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace InventoryManagment.Pages
{
    public class InventoryModel : PageModel
    {
        private readonly List<InventoryItem> _inventoryItems;
        private readonly IConfiguration _config;

        [BindProperty]
        public List<InventoryItem> InventoryItems { get; set; }

        [BindProperty]
        public string AiSummary { get; set; }

        public InventoryModel(List<InventoryItem> inventoryItems, IConfiguration config)
        {
            _inventoryItems = inventoryItems;
            _config = config;
        }

        // Method to handle GET request and display inventory
        public void OnGet()
        {
            InventoryItems = _inventoryItems;
        }

        // Handle POST for deleting an item
        public IActionResult OnPostDelete(int id)
        {
            var itemToDelete = _inventoryItems.FirstOrDefault(item => item.Id == id);
            if (itemToDelete != null)
            {
                _inventoryItems.Remove(itemToDelete);
            }

            return RedirectToPage(); // Refresh the page after deletion
        }

        // Handle POST for adding an item
        public IActionResult OnPost(string name, int quantity, string category, string description, DateTime? dueDate, string status)
        {
            var newItem = new InventoryItem
            {
                Name = name,
                Quantity = quantity,
                Category = category,
                Description = description,
                DueDate = dueDate,
                Status = status
            };

            _inventoryItems.Add(newItem);

            return RedirectToPage(); // Refresh the page after adding an item
        }

        // Method to handle AI summarization for "InStock" items
        public async Task<IActionResult> OnPostAiSummarizeAsync()
        {
            var apiKey = _config["Gemini:ApiKey"]; // Fetch API key from appsettings
            var http = new HttpClient();
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            // Get items that are "InStock"
            var inStockItems = _inventoryItems.Where(item => item.Status == "InStock").ToList();

            if (!inStockItems.Any())
            {
                AiSummary = "There are no items in stock!";
                return Page(); // If no items are in stock, return the page with the message
            }

            // Construct the prompt to send to Gemini
            var prompt = "Here are the items in stock:\n\n";
            foreach (var item in inStockItems)
            {
                prompt += $"- {item.Name}: {item.Quantity} units\n";
            }

            // Request body for Gemini
            var requestBody = new
            {
                contents = new[] {
            new {
                parts = new[] {
                    new {
                        text = $@"
                        You are a helpful assistant. Summarize the following items in stock for the user in a clear, motivational tone:

                        Items in stock:
                        {prompt}

                        Keep the summary brief and focus on the positive."
                    }
                }
            }
        }
            };

            try
            {
                // Send request to Gemini API
                var response = await http.PostAsJsonAsync(url, requestBody);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var root = JsonDocument.Parse(json);
                    var summaryText = root.RootElement.GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    AiSummary = summaryText ?? "AI couldn't generate a valid summary.";
                }
                else
                {
                    AiSummary = "Error: " + json;
                }
            }
            catch (Exception ex)
            {
                AiSummary = "An error occurred while generating the AI summary: " + ex.Message;
            }

            return Page(); // Return the page to show the AI summary
        }

    }
}
