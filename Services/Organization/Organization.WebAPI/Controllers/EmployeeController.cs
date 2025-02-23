using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Organization.Application.Warehouses.Queries.GetWarehouseQuery;
using Organization.WebAPI.DTOs.User;
using Shared.ResultTypes;
using Shared.Services;
using System.Text;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : BaseController
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISharedIdentityService _sharedIdentityService;

    public EmployeeController(IHttpClientFactory httpClientFactory, ISharedIdentityService sharedIdentityService)
    {
        _httpClientFactory = httpClientFactory;
        _sharedIdentityService = sharedIdentityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var client = _httpClientFactory.CreateClient("emp");
        string companyId = _sharedIdentityService.GetCompanyId;
        var response = await client.GetAsync($"http://104.248.36.17:5001/api/Employees?companyId={companyId}");

        var employees = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        foreach (var employee in employees)
        {
            if(employee.WarehouseId != null)
            {
                employee.WarehouseName = (await Mediator.Send(new GetWarehouse(employee.WarehouseId))).Name;
            }
            else
            {
                employee.WarehouseName = "N/A";
            }
        }
        var result = Response<List<UserDto>>.Success(employees, 200);
        return Ok(result);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        string id = _sharedIdentityService.GetUserId;
        var client = _httpClientFactory.CreateClient("employees");
        var response = await client.GetAsync($"http://104.248.36.17:5001/api/Users/{id}");
        var employee = await response.Content.ReadFromJsonAsync<UserDto>();
        if (employee.WarehouseId != null)
        {
            employee.WarehouseName = (await Mediator.Send(new GetWarehouse(employee.WarehouseId))).Name;
        }
        else
        {
            employee.WarehouseName = "N/A";
        }
        var result = Response<UserDto>.Success(employee, 200);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateUserDto request)
    {
        var client = _httpClientFactory.CreateClient("employees");
        var jsondata = JsonConvert.SerializeObject(request);
        StringContent stringContent = new StringContent(jsondata, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"http://104.248.36.17:5001/api/Employees?companyId={_sharedIdentityService.GetCompanyId}", stringContent);

        Response<Shared.ResultTypes.NoContent> result;
        if (response.IsSuccessStatusCode)
        {
            result = Response<Shared.ResultTypes.NoContent>.Success(200);
        }
        else
        {
            result = Response<Shared.ResultTypes.NoContent>.Fail("İşçini artırmaq mümkün olmadı", 400);
        }
        return Ok(result);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignUserRole(string userId, string roleId)
    {
        var client = _httpClientFactory.CreateClient();
        string companyId = _sharedIdentityService.GetCompanyId;
        var response = await client.PostAsync($"http://104.248.36.17:5001/api/Roles/assign-role?userid={userId}&roleId={roleId}", null);

        Response<Shared.ResultTypes.NoContent> result;


        if (response.IsSuccessStatusCode)
        {
            result = Response<Shared.ResultTypes.NoContent>.Success(200);
        }
        else
        {
            result = Response<Shared.ResultTypes.NoContent>.Fail("Error oldu", 400);
        }
        return Ok(result);
    }

    [HttpPost("updatebranch")]
    public async Task<IActionResult> UpdateBranch(string userId, string branchId)
    {
        var client = _httpClientFactory.CreateClient("employees");
        var response = await client.PostAsync($"http://104.248.36.17:5001/api/users/UpdateBranch?userId={userId}&branchId={branchId}", null);

        Response<Shared.ResultTypes.NoContent> result;

        if (response.IsSuccessStatusCode)
        {
            result = Response<Shared.ResultTypes.NoContent>.Success(200);
        }
        else
        {
            result = Response<Shared.ResultTypes.NoContent>.Fail($"Error oldu {response}", 400);
        }
        return Ok(result);
    }

    [HttpGet("httpaccesor")]
    public async Task<IActionResult> GetHttpAccesor()
    {
        var content = _sharedIdentityService.GetUser;
        return Ok(content);
    }
}
