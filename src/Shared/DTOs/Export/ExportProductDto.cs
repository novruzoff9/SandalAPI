namespace Shared.DTOs.Export;

public class ExportProductDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellPrice { get; set; }
    public int Quantity { get; set; }
    public int Ordered { get; set; }
}

public class ExportCustomerDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}

public class ExportOrderDto
{
    public string Id { get; set; }
    public string CustomerName { get; set; }
    public string WarehouseName { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalProduct { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
}

public class ExportWarehouseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string GoogleMaps { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public int Capacity { get; set; }
    public int Workers { get; set; }
    public bool IsActive { get; set; }
}