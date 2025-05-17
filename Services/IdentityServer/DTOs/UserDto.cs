namespace IdentityServer.DTOs;

public class UserDto
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string? CompanyId { get; set; }
    public string WarehouseId { get; set; }
}

public class UserShowDto: UserDto
{
    public string Id { get; set; }
    public List<string> Roles { get; set; }
}

public class UserDetailedDto : UserShowDto
{
    public string HashedPassword { get; set; }
}
public class CreateUserDto : UserDto
{
    public string Password { get; set; }
}

public class UpdateUserDto : UserDto
{
    public string UserId { get; set; }
}

