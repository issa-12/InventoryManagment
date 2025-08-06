namespace InventoryManagment.Model
{
    public class InventoryItem
    {

            public int Id { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public DateTime? DueDate { get; set; }

            // Make sure the Status is initialized
            public string Status { get; set; } = "InStock";  // Default value can be InStock or any other status.
    }

}
