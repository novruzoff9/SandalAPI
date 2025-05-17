namespace IdentityServer.DTOs;

public class AssignUserToRoleDto
{
    public string UserId { get; set; }
    public string RoleId { get; set; }
}

public class AssignUserToWarehouseDto
{
    public string UserId { get; set; }
    public string WarehouseId { get; set; }
}
