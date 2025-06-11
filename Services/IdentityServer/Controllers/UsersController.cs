using FluentValidation;
using FluentValidation.Results;
using IdentityServer.DTOs;
using IdentityServer.Services;
using IdentityServer.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly ISharedIdentityService _sharedIdentityService;
    public UsersController(IUserService userService, IUserRoleService userRoleService, ISharedIdentityService sharedIdentityService)
    {
        _userService = userService;
        _userRoleService = userRoleService;
        _sharedIdentityService = sharedIdentityService;
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto userDto)
    {
        var user = await _userService.CreateUser(userDto);

        return Ok(user);
    }

    [HttpPost("assignrole")]
    public async Task<IActionResult> AssignUserToRole(AssignUserToRoleDto dto)
    {
        var result = await _userRoleService.AddUserToRoleAsync(dto.UserId, dto.RoleId);
        return Ok(result);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto userDto)
    {
        var updatedUser = await _userService.UpdateUser(userId, userDto);
        return Ok(updatedUser);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var role = await _userService.GetUsersAync();
        return Ok(role);
    }

    [Authorize]
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        string id = _sharedIdentityService.GetUserId;
        if (string.IsNullOrEmpty(id))
        {
            return Unauthorized($"İstifadəçi identifikasiyası tapılmadı. {id}");
        }
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var response = await _userService.ChangePassword(changePasswordDto);
        return Ok("Şifrə uğurla dəyişdirildi.");
    }
}
