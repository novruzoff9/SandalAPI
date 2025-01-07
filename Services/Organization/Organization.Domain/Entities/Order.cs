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
    public DateTime? Closed { get; set; }
    public string CompanyId { get; set; }
    public Company Company { get; set; }
    public List<OrderItem> Products { get; set; }
    public string WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
}

public class OrderItem : BaseEntity
{
    public string OrderId { get; set; }
    public Order Order { get; set; }
    public string ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
}
