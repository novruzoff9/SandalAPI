using Order.Application.Common.Interfaces;
using OrderService.Protos.Identity;

namespace Order.Infrastructure.Services.gRPC;

public class IdentityGrpcClient : IIdentityGrpcClient
{
    private readonly Identity.IdentityClient _identityClient;

    public IdentityGrpcClient(Identity.IdentityClient identityClient)
    {
        _identityClient = identityClient;
    }

    public async Task<string> GetUserEmailAsync(string userId)
    {
        GetEmployeeRequest request = new() { Id = userId };
        GetEmployeeResponse? response = await _identityClient.GetEmployeeAsync(request);
        if (response == null)
        {
            throw new Exception("User not found");
        }
        if (!response.Success)
        {
            throw new Exception(response.Message.ToString());
        }
        return response.Employee.Email;
    }

    public async Task<string> GetUserFullNameAsync(string userId)
    {
        GetEmployeeRequest request = new() { Id = userId };
        GetEmployeeResponse? response = new();
        try
        {
            response = await _identityClient.GetEmployeeAsync(request);
        }
        catch (Exception)
        {
            response.Employee = new();
            response.Success = true;
        }
        //TODO: Bununla bagli nese fikirles
        //if (response == null)
        //{
        //    throw new Exception("User not found");
        //}
        if(!response.Success)
        {
            throw new Exception(response.Message.ToString());
        }
        return response.Employee.Name;
    }
}
