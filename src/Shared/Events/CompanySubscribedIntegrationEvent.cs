using EventBus.Base.Events;

namespace Shared.Events
{
    public class CompanyAssignedPackIntegrationEvent : IntegrationEvent
    {
        public string CompanyId { get; private set; }
        public string PackageId { get; private set; }
        public string PackageName { get; private set; }
        public DateTime ExpiredTime { get; private set; }
        public CompanyAssignedPackIntegrationEvent(string companyId, string packageId, string packageName, DateTime expiredTime)
        {
            CompanyId = companyId;
            PackageId = packageId;
            PackageName = packageName;
            ExpiredTime = expiredTime;
        }
    }
}
