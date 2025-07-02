namespace Organization.Application.Common.Interfaces;

public interface IIdentityGrpcClient
{
    Task<int> GetEmployeeCountOfWarehouseAsync(string warehouseId);
}
