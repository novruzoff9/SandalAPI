using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using IdentityServer.Protos;
using Microsoft.AspNetCore.Mvc;
using Organization.Application.Warehouses.Queries.GetWarehouseQuery;
using Shared.Interceptors;
using Shared.ResultTypes;
using Shared.Services;

namespace Organization.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : BaseController
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISharedIdentityService _sharedIdentityService;
    private readonly IConfiguration _configuration;
    private readonly string _identityService;
    private readonly Identity.IdentityClient _identityClient;

    public EmployeeController(IHttpClientFactory httpClientFactory, ISharedIdentityService sharedIdentityService, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _sharedIdentityService = sharedIdentityService;
        _configuration = configuration;
        _identityService = _configuration["Services:IdentityService"] ?? "http://localhost:5001";
        string identityGrpcService = _configuration["Services:IdentityGrpcService"] ?? "http://localhost:5003";

        var channel = GrpcChannel.ForAddress($"{identityGrpcService}", new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Insecure
        });
        var callInvoker = channel.Intercept(new InternalRequestInterceptor());
        _identityClient = new Identity.IdentityClient(callInvoker);
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        string companyId = _sharedIdentityService.GetCompanyId;

        GetEmployeesResponse response = await _identityClient.GetEmployeesAsync(new GetEmployeesRequest
        {
            CompanyId = companyId
        });

        foreach (var employee in response.Employees)
        {
            employee.WarehouseName = employee.WarehouseId != "N/A"
                ? (await Mediator.Send(new GetWarehouse(employee.WarehouseId))).Name
                : "N/A";
        }
        var result = Response<List<Employee>>.Success(response.Employees.ToList(), 200);
        return Ok(result);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        string id = _sharedIdentityService.GetUserId;

        GetEmployeeResponse response = await _identityClient.GetEmployeeAsync(new GetEmployeeRequest
        {
            Id = id
        });

        if (!response.Success)
        {
            return BadRequest(response.Message);
        }
        var employee = response.Employee;

        employee.WarehouseName = employee.WarehouseId != "N/A"
            ? (await Mediator.Send(new GetWarehouse(employee.WarehouseId))).Name
            : "N/A";


        var result = Response<Employee>.Success(employee, 200);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateEmployeeRequest request)
    {
        string companyId = _sharedIdentityService.GetCompanyId;
        var response = await _identityClient.CreateEmployeeAsync(new CreateEmployeeRequest
        {
            Name = request.Name,
            Email = request.Email,
            CompanyId = companyId,
            WarehouseId = request.WarehouseId,
            Password = request.Password
        });

        Response<Shared.ResultTypes.NoContent> result;

        result = response.Success
            ? Response<Shared.ResultTypes.NoContent>.Success(200) 
            : Response<Shared.ResultTypes.NoContent>.Fail(response.Message, 400);

        return Ok(result);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignUserRole(string userId, string roleId)
    {
        var client = _httpClientFactory.CreateClient();
        string companyId = _sharedIdentityService.GetCompanyId;
        var response = await client.PostAsync($"{_identityService}/api/Roles/assign-role?userid={userId}&roleId={roleId}", null);

        Response<Shared.ResultTypes.NoContent> result;


        result = response.IsSuccessStatusCode
            ? Response<Shared.ResultTypes.NoContent>.Success(200)
            : Response<Shared.ResultTypes.NoContent>.Fail("Error oldu", 400);
        return Ok(result);
    }

    [HttpPost("updatebranch")]
    public async Task<IActionResult> UpdateBranch(string userId, string branchId)
    {
        var client = _httpClientFactory.CreateClient("employees");
        var response = await client.PostAsync($"{_identityService}/api/users/UpdateBranch?userId={userId}&branchId={branchId}", null);

        Response<Shared.ResultTypes.NoContent> result;

        result = response.IsSuccessStatusCode
            ? Response<Shared.ResultTypes.NoContent>.Success(200)
            : Response<Shared.ResultTypes.NoContent>.Fail($"Error oldu {response}", 400);
        return Ok(result);
    }

    [HttpGet("httpaccesor")]
    public IActionResult GetHttpAccesor()
    {
        var content = _sharedIdentityService.GetUser;
        return Ok(content);
    }
}
