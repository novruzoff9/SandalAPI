namespace IdentityServer.Models;

public class IdentityRole
{
    public string Id { get; private set; }
    public string RoleName { get; private set; }
    public string NormalizedName { get; private set; }
    public IReadOnlyList<UserRole> Users { get; set; } = new List<UserRole>();
    public IdentityRole(string roleName)
    {
        Id = Guid.NewGuid().ToString();
        RoleName = roleName;
        NormalizedName = roleName.Trim().ToLower();
    }
}
