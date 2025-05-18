using IdentityServer.DTOs;
using IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.ResultTypes;
using Shared.Services;

namespace IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IUserService _userService;
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IUserRoleService _userRoleService;

    public EmployeesController(IEmployeeService employeeService, ISharedIdentityService sharedIdentityService, IUserService userService, IUserRoleService userRoleService)
    {
        _employeeService = employeeService;
        _sharedIdentityService = sharedIdentityService;
        _userService = userService;
        _userRoleService = userRoleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _employeeService.GetEmployeesAsync();
        var response = Response<List<UserShowDto>>.Success(employees, 200);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> AddEmployee(CreateUserDto employeeDto)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        employeeDto.CompanyId = companyId;
        UserShowDto employee = await _userService.CreateUser(employeeDto);
        var response = Response<UserShowDto>.Success(employee, 201);
        return Ok(response);
    }

    [HttpPost("update-branch")]
    public async Task<IActionResult> UpdateEmployeeWarehouse(AssignUserToWarehouseDto request)
    {
        var result = await _employeeService.ChangeWarehouse(request.UserId, request.WarehouseId);
        if (result)
        {
            return Ok(Response<string>.Success("Anbar dəyişdirildi", 200));
        }
        return BadRequest(Response<string>.Fail("Anbar dəyişdirilmədi", 400));
    }
    [HttpPost("assignrole")]
    public async Task<IActionResult> AssignUserToRole(AssignUserToRoleDto dto)
    {
        var result = await _userRoleService.AddUserToRoleAsync(dto.UserId, dto.RoleId);
        return Ok(result);
    }
}
