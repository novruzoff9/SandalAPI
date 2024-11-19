using Organization.Domain.Common;

namespace Organization.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string CompanyID { get; set; }
    public Company Company { get; set; }
    public List<Shelf> Shelves { get; set; }
}
