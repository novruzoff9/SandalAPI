using Azure.Core;
using FluentValidation;
using IdentityServer.Context;
using IdentityServer.DTOs;
using IdentityServer.Models;
using IdentityServer.Validations;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Services;


public interface IRoleService
{
    Task<IdentityRole> CreateRoleAsync(CreateRoleDto request);
    Task<List<IdentityRole>> GetRolesAsync();
}

public class RoleService : IRoleService
{
    private readonly IdentityDbContext _context;
    private readonly IValidator<CreateRoleDto> _createRoleValidator;

    public RoleService(IdentityDbContext context, IValidator<CreateRoleDto> createRoleValidator)
    {
        _context = context;
        _createRoleValidator = createRoleValidator;
    }

    public async Task<IdentityRole> CreateRoleAsync(CreateRoleDto request)
    {
        var valResult = _createRoleValidator.Validate(request);
        if (!valResult.IsValid)
        {
            throw new ValidationException(valResult.Errors);
        }
        string normalizedRoleName = request.Name.Trim().ToLower();
        var role = new IdentityRole(request.Name);
        var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName);
        if (existingRole != null)
        {
            throw new Exception("Rol artıq mövcuddur");
        }
        await _context.Roles.AddAsync(role);
        _context.SaveChanges();
        return role;
    }

    public Task<List<IdentityRole>> GetRolesAsync()
    {
        var roles = _context.Roles.ToListAsync();
        return roles;
    }
}
