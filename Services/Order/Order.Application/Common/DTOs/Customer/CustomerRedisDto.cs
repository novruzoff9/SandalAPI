namespace Order.Application.Common.DTOs.Customer;

public class CustomerRedisDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedTime { get; set; }

    public CustomerRedisDto(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        CreatedTime = DateTime.Now;
    }
}
