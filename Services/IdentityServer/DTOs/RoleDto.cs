namespace IdentityServer.DTOs;

public class CreateRoleDto
{
    public string Name { get; set; }
}

public class UpdateRoleDto
{
    public string Name { get; set; }
    public string RoleId { get; set; }
}

public class RoleDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
}