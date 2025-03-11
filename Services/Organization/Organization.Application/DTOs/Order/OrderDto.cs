namespace Organization.Application.DTOs.Order;
public class OrderDto
{
    public string WarehouseId { get; set; } = null!;
    public List<OrderItemDto> OrderItems { get; set; }
}

public class OrderItemDto
{
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
}

public class OrderShowDto
{
    public string Id { get; set; } = null!;
    public string Warehouse { get; set; } = null!;
    public string Opened { get; set; } = null!;
    public string OpenedBy { get; set; } = null!;
    public string? Closed { get; set; }
    public string? ClosedBy { get; set; }
    public string Status { get; set; } = null!;
}

public class OrderItemShowDto
{
    public string Id { get; set; } = null!;
    public int Quantity { get; set; }
    public string Name { get; set; } = null!;
    public string ShelfCode { get; set; }
}
public class CompleteOrderRequest
{
    public Dictionary<string, int> Products { get; set; }
}