using IdentityModel;
using IdentityServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByEmailAsync(context.UserName);

            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
                return;
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, context.Password);
            if (!passwordCheck)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
                return;
            }

            // Kullanıcının rollerini al
            var roles = await _userManager.GetRolesAsync(user);

            // Claim listesi oluştur ve roller ekle
            var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString()), // Kullanıcı kimliği
            new Claim("email", user.Email),        // Kullanıcı e-posta
            new Claim("company", user.CompanyId ?? "Sandal"),
            new Claim("warehouse", user.WarehouseId == null ? "noBranch" : user.WarehouseId)
        };

            // Rolleri ekle
            claims.AddRange(roles.Select(role => new Claim("roles", role)));

            // Kullanıcı doğrulama başarılı, ek claim'lerle birlikte token oluşturulacak
            context.Result = new GrantValidationResult(
                subject: user.Id.ToString(),
                authenticationMethod: OidcConstants.AuthenticationMethods.Password,
                claims: claims // Claim listesi
            );
        }
    }
}
