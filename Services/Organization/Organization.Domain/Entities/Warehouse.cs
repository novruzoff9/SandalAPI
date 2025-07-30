using Organization.Domain.Common;
using Organization.Domain.ValueObjects;

namespace Organization.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? GoogleMaps { get; set; } 
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string CompanyID { get; set; } = null!;
    public Company Company { get; set; }
    public List<Shelf>? Shelves { get; set; }

    private Warehouse() { }

    public Warehouse(string name, string city, string state, string street, string zipCode, string companyId, string? googleMaps = null)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        GoogleMaps = googleMaps ?? string.Empty;
        City = city;
        State = state;
        Street = street;
        ZipCode = zipCode;
        CompanyID = companyId;
    }

    public void Update(string name, string city, string state, string street, string zipCode, string? googleMaps = null)
    {
        Name = name;
        GoogleMaps = googleMaps ?? string.Empty;
        City = city;
        State = state;
        Street = street;
        ZipCode = zipCode;
    }
}
