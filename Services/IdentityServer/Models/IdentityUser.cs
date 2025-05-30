using System.Text.RegularExpressions;

namespace IdentityServer.Models;

public class IdentityUser
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"^\+994\d{2}-\d{3}-\d{2}-\d{2}$", RegexOptions.Compiled);

    public string Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string NormalizedEmail { get; set; }
    public string PhoneNumber { get; private set; }
    public string HashedPassword { get; private set; }
    public string CompanyId { get; set; }
    public string WarehouseId { get; set; }
    public IReadOnlyList<UserRole> Roles { get; set; } = new List<UserRole>();

    private IdentityUser() { }

    public IdentityUser(string firstName, string lastName, string email, string phoneNumber, string password, string companyId, string? warehouseId)
    {
        if (!EmailRegex.IsMatch(email))
            throw new FormatException("Email format is invalid.");

        if (!PhoneRegex.IsMatch(phoneNumber))
            throw new FormatException("Telefon nömrəi +994xx-xxx-xx-xx formatında olmalıdır");

        Id = Guid.NewGuid().ToString();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        NormalizedEmail = email.Trim().ToLower();
        PhoneNumber = phoneNumber;
        HashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        CompanyId = companyId;
        WarehouseId = warehouseId ?? "N/A";
    }

    public void Update(string firstName, string lastName, string email, string phoneNumber)
    {
        if (!EmailRegex.IsMatch(email))
            throw new FormatException("Email format is invalid.");

        if (!PhoneRegex.IsMatch(phoneNumber))
            throw new FormatException("Phone number must be in +994xx-xxx-xx-xx format.");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        NormalizedEmail = email.Trim().ToLower();
        PhoneNumber = phoneNumber;
    }

    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("New password cannot be empty.", nameof(newPassword));
        HashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
    }
}
