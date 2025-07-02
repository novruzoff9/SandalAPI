using IdentityServer.Protos;
using Organization.Application.Common.Interfaces;

namespace Organization.Infrastructure.Services.gRPC;

public class IdentityGrpcClient : IIdentityGrpcClient
{
    private readonly Identity.IdentityClient _client;
    public IdentityGrpcClient(Identity.IdentityClient client)
    {
        _client = client;
    }
    public async Task<int> GetEmployeeCountOfWarehouseAsync(string warehouseId)
    {
        GetWarehouseEmpCountRequest request = new() { WarehouseId = warehouseId };
        var response = await _client.GetWarehouseEmpCountAsync(request);
        return response.Count;
    }
}
