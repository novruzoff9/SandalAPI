namespace Shared.Events;

public class CompanyAssignedPackIntegrationEvent : IntegrationEvent
{
    public string CompanyId { get; init; }
    public string PackageId { get; init; }
    public string PackageCode { get; init; }
    public string PackageName { get; init; }
    public DateTime ExpiredTime { get; init; }
    public CompanyAssignedPackIntegrationEvent(string companyId, string packageId, string packageCode, string packageName, DateTime expiredTime)
    {
        CompanyId = companyId;
        PackageId = packageId;
        PackageCode = packageCode;
        PackageName = packageName;
        ExpiredTime = expiredTime;
    }
}
