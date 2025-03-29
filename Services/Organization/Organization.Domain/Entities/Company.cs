using Organization.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; private set; }
    public string LogoUrl { get; private set; }
    public string Description { get; private set; }
    public List<Warehouse>? Warehouses { get; set; }
    public List<Product>? Products { get; set; }
    public List<Customer>? Customers { get; set; }

    public Company(string name, string logoUrl, string description)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        LogoUrl = logoUrl;
        Description = description;
    }

    public void Update(string name, string logoUrl, string description)
    {
        Name = name;
        LogoUrl = logoUrl;
        Description = description;
    }

    public void UpdateLogo(string logoUrl)
    {
        LogoUrl = logoUrl;
    }
}
