using Organization.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; }
    public string LogoUrl { get; set; }
    public string Description { get; set; }
    public List<Warehouse> Warehouses { get; set; }
    public List<Product> Products { get; set; }
    public List<Order> Orders { get; set; }
}
