namespace Organization.Application.DTOs.Company;

public class CompanyDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Warehouses { get; set; }
    public string Subscription { get; set; }
}

public class CompanyDetailsDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Warehouses { get; set; }
    public string Subscription { get; set; }
    public List<string> WarehousesList { get; set; }
}

public class CompanySubscriptionRedisDto
{
    public CompanySubscriptionRedisDto(string packageId, string packageName, DateTime expiredTime)
    {
        PackageId = packageId;
        PackageName = packageName;
        ExpiredTime = expiredTime;
    }

    public string PackageId { get; private set; }
    public string PackageName { get; private set; }
    public DateTime ExpiredTime { get; private set; }
}