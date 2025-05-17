using EventBus.Base.Abstraction;
using Shared.Events.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscription.Application.IntegrationEvent.Handlers;

public class CompanyAssignedPackageIntegrationEventHandler : IIntegrationEventHandler<CompanyAssignedPackageIntegrationEvent>
{
    public Task Handle(CompanyAssignedPackageIntegrationEvent @event, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
