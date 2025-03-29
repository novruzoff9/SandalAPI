using System.Text.RegularExpressions;

namespace Organization.Domain.Common;

public abstract class PersonEntity : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }

    protected PersonEntity(string firstName, string lastName, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name cannot be empty");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name cannot be empty");
        if (!IsValidEmail(email)) throw new ArgumentException("Invalid email format");
        if (!IsValidPhone(phone)) throw new ArgumentException("Invalid phone number");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
    }

    public string FullName => $"{FirstName} {LastName}";

    private bool IsValidEmail(string email) =>
        Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    private bool IsValidPhone(string phone) =>
        Regex.IsMatch(phone, @"^\+994(50|51|55|70|77|99|10|60|12)-\d{3}-\d{2}-\d{2}$");


    public void UpdateContactInfo(string email, string phone)
    {
        if (!IsValidEmail(email)) throw new ArgumentException("Invalid email format");
        if (!IsValidPhone(phone)) throw new ArgumentException("Invalid phone number");

        Email = email;
        Phone = phone;
    }
}

