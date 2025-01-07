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
    public DateTime Opened { get; set; }
    public DateTime? Closed { get; set; }
    public string Status { get; set; }
}

public class OrderItemShowDto
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; }
    public string ShelfCode { get; set; }
}
