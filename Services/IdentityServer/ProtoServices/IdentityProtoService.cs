using Google.Protobuf;
using Grpc.Core;
using IdentityServer.DTOs;
using IdentityServer.Protos;
using IdentityServer.Services;
using Microsoft.AspNetCore.Identity;

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
}
