using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Common.Models.Warehouse;

public class WarehouseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? GoogleMaps { get; set; }
    public string City { get; set; }
}
