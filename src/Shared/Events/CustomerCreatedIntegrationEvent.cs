using EventBus.Base.Events;

namespace Shared.Events;

public class CustomerCreatedIntegrationEvent : IntegrationEvent
{
    public string CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CompanyId { get; set; }
}
