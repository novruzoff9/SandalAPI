namespace Organization.Application.DTOs.Shelf;
public class ShelfDTO
{
    public string Id { get; set; }
    public string Code { get; set; }
    public int ItemsCount { get; set; }
}

public class ShelfProductDTO
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public string ShelfId { get; set; }
    public string ShelfCode { get; set; }
    public string? ImageUrl { get; set; }
}

