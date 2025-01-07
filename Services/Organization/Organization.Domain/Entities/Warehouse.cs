using Organization.Domain.Common;
using Organization.Domain.ValueObjects;

namespace Organization.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; }
    public string? GoogleMaps { get; set; } 
    public string City { get; set; }
    public string State { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string CompanyID { get; set; }
    public Company Company { get; set; }
    public List<Shelf> Shelves { get; set; }
    public List<Order> Orders { get; set; }
}
