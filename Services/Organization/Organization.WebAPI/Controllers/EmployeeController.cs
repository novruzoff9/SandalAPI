using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Organization.Application.Warehouses.Queries.GetWarehouseQuery;
using Organization.Application.DTOs.User;
using Shared.ResultTypes;
using Shared.Services;
using System.Text;
using Grpc.Net.Client;
using IdentityServer.Protos;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Shared.Interceptors;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : BaseController
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IConfiguration _configuration;
    private readonly string _identityService;
    private readonly string _identityGrpcService;

    public EmployeeController(IHttpClientFactory httpClientFactory, ISharedIdentityService sharedIdentityService, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _sharedIdentityService = sharedIdentityService;
        _configuration = configuration;
        _identityService = _configuration["Services:IdentityService"] ?? "http://localhost:5001";
        _identityGrpcService = _configuration["Services:IdentityGrpcService"] ?? "http://localhost:5003";
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var channel = GrpcChannel.ForAddress($"{_identityGrpcService}", new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Insecure
        });

        var callInvoker = channel.Intercept(new InternalRequestInterceptor());
        var identityClient = new Identity.IdentityClient(callInvoker);

        GetEmployeesResponse response = await identityClient.GetEmployeesAsync(new GetEmployeesRequest
        {
            CompanyId = companyId
        });


        foreach (var employee in response.Employees)
        {
            if (employee.WarehouseId != "N/A")
            {
                employee.WarehouseName = (await Mediator.Send(new GetWarehouse(employee.WarehouseId))).Name;
            }
            else
            {
                employee.WarehouseName = "N/A";
            }
        }
        var result = Response<List<Employee>>.Success(response.Employees.ToList(), 200);
        return Ok(result);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        string id = _sharedIdentityService.GetUserId;
        var client = _httpClientFactory.CreateClient("employees");
        client.DefaultRequestHeaders.Add("X-Internal-Request", "true");
        var response = await client.GetAsync($"{_identityService}/api/Users/{id}");
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
    public async Task<IActionResult> Add(CreateEmployeeRequest request)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var channel = GrpcChannel.ForAddress($"{_identityGrpcService}", new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Insecure
        });
        var identityClient = new Identity.IdentityClient(channel);
        var response = await identityClient.CreateEmployeeAsync(new CreateEmployeeRequest
        {
            Name = request.Name,
            Email = request.Email,
            CompanyId = companyId,
            WarehouseId = request.WarehouseId,
            Password = request.Password
        });

        Response<Shared.ResultTypes.NoContent> result;
        if (response.Success)
        {
            result = Response<Shared.ResultTypes.NoContent>.Success(200);
        }
        else
        {
            result = Response<Shared.ResultTypes.NoContent>.Fail(response.Message, 400);
        }
        return Ok(result);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignUserRole(string userId, string roleId)
    {
        var client = _httpClientFactory.CreateClient();
        string companyId = _sharedIdentityService.GetCompanyId;
        var response = await client.PostAsync($"{_identityService}/api/Roles/assign-role?userid={userId}&roleId={roleId}", null);

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
        var response = await client.PostAsync($"{_identityService}/api/users/UpdateBranch?userId={userId}&branchId={branchId}", null);

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
