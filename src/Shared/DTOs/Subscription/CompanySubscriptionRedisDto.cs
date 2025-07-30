namespace Shared.DTOs.Subscription;

public class CompanySubscriptionRedisDto
{
    public string PackageId { get; set; }
    public string PackageName { get; set; }
    public string PackageCode { get; set; }
    public DateTime ExpiredTime { get; set; }
    public CompanySubscriptionRedisDto(string packageId, string packageName, string packageCode, DateTime expiredTime)
    {
        PackageId = packageId;
        PackageName = packageName;
        PackageCode = packageCode;
        ExpiredTime = expiredTime;
    }
}
