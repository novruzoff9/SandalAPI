namespace Organization.Application.DTOs.Product;
public class ExportProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellPrice { get; set; }
    public int Quantity { get; set; }
    public int Ordered { get; set; }
}
