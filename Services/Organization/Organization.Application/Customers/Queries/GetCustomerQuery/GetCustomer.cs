
namespace Organization.Application.Customers.Queries.GetCustomerQuery;

public record GetCustomerQuery(string Id) : IRequest<Customer>;

public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, Customer>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return customer;
    }
}

