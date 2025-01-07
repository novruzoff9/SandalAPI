using IdentityServer.DTOs.Role;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            roleName = roleName.ToLower();
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest("Role already exists");
            }

            var result = await _roleManager.CreateAsync(new ApplicationRole(roleName));

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser(string userId, string roleId)
        {
            // Kullanıcıyı bulalım
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Rolü bulalım
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            var oldRoles = await _userManager.GetRolesAsync(user);

            // Kullanıcıyı role atayalım
            await _userManager.RemoveFromRolesAsync(user, oldRoles);
            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User assigned to role successfully");
        }

        [HttpPost("delete-role")]
        public async Task<IActionResult> RemoveRole(string userId, string roleId)
        {
            // Kullanıcıyı bulalım
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Rolü bulalım
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            // Rolu kullanicidan silelim
            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User removed from role successfully");
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRolesOfUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

    }
}
