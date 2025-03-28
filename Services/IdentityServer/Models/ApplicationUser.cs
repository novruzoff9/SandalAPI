using Microsoft.AspNetCore.Identity;
using System;

namespace IdentityServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<string>
    {
        public string CompanyId { get; set; }
        public string WarehouseId { get; set; }
    }

    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string Name) : this()
        {
            this.Name = Name;
        }
    }
}
