using Organization.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Domain.Entities;

public class Customer : PersonEntity
{
    public string CompanyId { get; set; }
    public Company? Company { get; set; }
    public Customer(string firstName, string lastName, string email, string phone, string companyId) : base(firstName, lastName, email, phone) {
        Id = Guid.NewGuid().ToString();
        CompanyId = companyId;
    }
}
