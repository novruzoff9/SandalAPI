using EventBus.Base.Abstraction;
using Organization.Domain.ValueObjects;
using Shared.Events;

namespace Organization.Application.Customers.Commands.CreateCustomerCommand;

public record CreateCustomerCommand(string FirstName, string LastName, string Email, string Phone, 
    Address Address) : IRequest<bool>;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ISharedIdentityService _identityService;
    private readonly IEventBus _eventBus;

    public CreateCustomerCommandHandler(IApplicationDbContext context, ISharedIdentityService identityService, IEventBus eventBus)
    {
        _context = context;
        _identityService = identityService;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(CreateCustomerCommand));
        string companyId = _identityService.GetCompanyId;

        var customer = new Customer(request.FirstName, request.LastName, request.Email, request.Phone, companyId, request.Address);

        await _context.Customers.AddAsync(customer, cancellationToken);

        var success = await _context.SaveChangesAsync(cancellationToken) > 0;

        CustomerCreatedIntegrationEvent @event = new()
        {
            CustomerId = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            CompanyId = companyId
        };

        _eventBus.Publish(@event);

        return success;
    }
}

