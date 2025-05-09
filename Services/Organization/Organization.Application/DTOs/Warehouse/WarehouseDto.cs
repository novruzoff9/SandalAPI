using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.DTOs.Warehouse;

public class WarehouseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Shelves { get; set; }
    public string? GoogleMaps { get; set; }
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Street { get; set; }
    public string ZipCode { get; set; }
}
