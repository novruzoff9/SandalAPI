using IdentityServer.DTOs;
using IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleDto role)
    {
        var result = await _roleService.CreateRoleAsync(role);
        if (result is not null)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest("Failed to create role.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleService.GetRolesAsync();
        return Ok(roles);
    }
}
