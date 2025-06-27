namespace Order.Application.Common.Interfaces;

public interface ICustomerService
{
    Task<string> GetCustomerFullNameAsync(string customerId);
}
