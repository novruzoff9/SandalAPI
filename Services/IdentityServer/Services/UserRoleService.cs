using IdentityServer.Context;
using IdentityServer.Exceptions;
using IdentityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Services;


public interface IUserRoleService
{
    Task<string> GetUserRole(string userId);
    Task<bool> AddUserToRoleAsync(string userId, string roleId);
    Task<bool> RemoveUserFromRole(string userId, string roleId);
}
public class UserRoleService : IUserRoleService
{
    private readonly IdentityDbContext _context;

    public UserRoleService(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddUserToRoleAsync(string userId, string roleId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("İstifadəçi tapılmadı");
        }

        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
        {
            throw new NotFoundException("Rol tapılmadı");
        }

        var userCurrentRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
        if (userCurrentRole != null && userCurrentRole.RoleId == roleId)
        {
            throw new ConflictException("İstifadəçi bu rola sahibdir");
        }
        else
        {
            if (userCurrentRole != null)
            {
                userCurrentRole.Revoke();
            }
            var userRole = new UserRole(userId, roleId);
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public async Task<string> GetUserRole(string userId)
    {
        var userRole = await _context.UserRoles
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && !ur.Revoked.HasValue);
        if (userRole != null)
        {
            return userRole.Role.RoleName;
        }
        else
        {
            return null;
            //throw new NotFoundException("İstifadəçi rolu tapılmadı");
        }
    }

    public async Task<bool> RemoveUserFromRole(string userId, string roleId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("İstifadəçi tapılmadı");
        }
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
        {
            throw new NotFoundException("Rol tapılmadı");
        }
        var userCurrentRole = _context.UserRoles
            .FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (userCurrentRole != null)
        {
            userCurrentRole.Revoke();
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            throw new Exception("İstifadəçi belə bir rola sahib deyil");
        }
    }

}
