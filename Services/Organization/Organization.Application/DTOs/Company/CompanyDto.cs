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