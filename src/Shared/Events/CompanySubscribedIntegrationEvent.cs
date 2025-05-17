using EventBus.Base.Events;

namespace Shared.Events
{
    public class CompanyAssignedPackageIntegrationEvent : IntegrationEvent
    {
        public string CompanyId { get; private set; }
        public string PackageId { get; private set; }
        public CompanyAssignedPackageIntegrationEvent(string companyId, string packageId)
        {
            CompanyId = companyId;
            PackageId = packageId;
        }
    }
}
