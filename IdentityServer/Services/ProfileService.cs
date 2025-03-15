using IdentityServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            if (user == null)
            {
                return;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(role => new Claim("roles", role)).ToList();
            context.IssuedClaims.AddRange(roleClaims);

            var companyClaim = new Claim("company", user.CompanyId ?? "Sandal");
            context.IssuedClaims.Add(companyClaim);

            var branchClaim = new Claim("warehouse", user.WarehouseId ?? "NoBranch");
            context.IssuedClaims.Add(branchClaim);

            var emailClaim = new Claim("email", user.Email);
            context.IssuedClaims.Add(emailClaim);

            var nameClaim = new Claim("name", user.UserName);
            context.IssuedClaims.Add(nameClaim);

            var idClaim = new Claim("id", user.Id);
            context.IssuedClaims.Add(idClaim);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
