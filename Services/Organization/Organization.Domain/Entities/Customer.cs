using Organization.Domain.Common;
using Organization.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Domain.Entities;

public class Customer : PersonEntity
{
    public string CompanyId { get; set; }
    public Address Address { get; set; }
    public Company? Company { get; set; }


    private Customer() : base() { }
    public Customer(string firstName, string lastName, string email, string phone, string companyId, Address address) : base(firstName, lastName, email, phone) {
        Id = Guid.NewGuid().ToString();
        CompanyId = companyId;
        Address = address;
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
    }
}
