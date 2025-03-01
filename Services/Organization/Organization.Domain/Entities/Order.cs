using Organization.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Domain.Entities;

public class Order : BaseEntity
{
    public DateTime Opened { get; set; }
    public string OpenedBy { get; set; } = null!;
    public DateTime? Closed { get; set; }
    public string? ClosedBy { get; set; }
    public string CompanyId { get; set; } = null!;
    public Company? Company { get; set; }
    public List<OrderItem>? Products { get; set; }
    public string WarehouseId { get; set; } = null!;
    public Warehouse? Warehouse { get; set; }
}

public class OrderItem : BaseEntity
{
    public string OrderId { get; set; } = null!;
    public Order? Order { get; set; }
    public string ProductId { get; set; } = null!;
    public Product? Product { get; set; }
    public int Quantity { get; set; }
}
