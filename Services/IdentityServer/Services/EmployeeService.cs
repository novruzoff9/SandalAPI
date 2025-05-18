using IdentityServer.Context;
using IdentityServer.DTOs;
using IdentityServer.Exceptions;
using Microsoft.EntityFrameworkCore;
using Shared.Services;

namespace IdentityServer.Services;

public interface IEmployeeService
{
    Task<List<UserShowDto>> GetEmployeesAsync();
    Task<UserShowDto> GetEmployeeAsync(string id);
    Task<bool> ChangeWarehouse(string userId, string warehouseId);
}

public class EmployeeService : IEmployeeService
{
    private readonly IdentityDbContext _context;
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IUserRoleService _userRoleService;

    public EmployeeService(IdentityDbContext context, ISharedIdentityService sharedIdentityService, IUserRoleService userRoleService)
    {
        _context = context;
        _sharedIdentityService = sharedIdentityService;
        _userRoleService = userRoleService;
    }

    public async Task<bool> ChangeWarehouse(string userId, string warehouseId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if(user == null)
        {
            throw new NotFoundException("İstifadəçi tapılmadı");
        }
        var companyId = _sharedIdentityService.GetCompanyId;
        if (user.CompanyId != companyId)
        {
            throw new NotFoundException("İstifadəçi tapılmadı");
        }
        if (user.WarehouseId == warehouseId)
        {
            throw new ConflictException("İstifadəçi artıq bu anbarda çalışır");
        }
        user.WarehouseId = warehouseId;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<UserShowDto> GetEmployeeAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<UserShowDto>> GetEmployeesAsync()
    {
        var companyId = _sharedIdentityService.GetCompanyId;
        var employees = await _context.Users.Where(x=>x.CompanyId == companyId).ToListAsync();
        var employeeDtos = employees.Select(x => new UserShowDto
        {
            Id = x.Id,
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName,
            PhoneNumber = x.PhoneNumber,
            WarehouseId = x.WarehouseId,
            Roles = _userRoleService.GetUserRole(x.Id).Result != null ? new List<string> { _userRoleService.GetUserRole(x.Id).Result } : new List<string>()
        }).ToList();
        return employeeDtos;
    }
}
