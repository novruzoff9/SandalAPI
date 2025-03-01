namespace Organization.WebAPI.DTOs.Order;

public class OrderDto
{
    public string WarehouseId { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}

public class OrderItemDto
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}

public class OrderShowDto
{
    public string Id { get; set; }
    public string Warehouse { get; set; }
    public string Opened { get; set; }
    public string? Closed { get; set; }
    public string Status { get; set; }
}

public class OrderItemShowDto
{
    public string Id { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; }
    public string ShelfCode { get; set; }
}
