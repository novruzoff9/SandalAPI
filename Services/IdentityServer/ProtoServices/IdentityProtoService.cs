using Grpc.Core;
using IdentityServer.Protos;
using IdentityServer.Services;

namespace IdentityServer.ProtoServices;

public class IdentityProtoService : Identity.IdentityBase
{
    private readonly IUserService _userService;

    public IdentityProtoService(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<GetEmployeeResponse> GetEmployee(GetEmployeeRequest request, ServerCallContext context)
    {
        GetEmployeeResponse response = new GetEmployeeResponse();
        var user = await _userService.GetUserByIdAsync(request.Id);
        response.Employee = new Employee
        {
            Id = user.Id,
            Name = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            CompanyId = user.CompanyId ?? string.Empty,
            WarehouseId = user.WarehouseId
        };
        response.Success = true;
        return response;
    }

    public override async Task<GetWarehouseEmpCountResponse> GetWarehouseEmpCount(GetWarehouseEmpCountRequest request, ServerCallContext context)
    {
        GetWarehouseEmpCountResponse response = new GetWarehouseEmpCountResponse();
        var users = await _userService.GetUsersAync();
        var count = users.Count(u => u.WarehouseId == request.WarehouseId);
        response.Count = count;
        response.Success = true;
        return response;
    }
}
