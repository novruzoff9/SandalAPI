namespace IdentityServer.DTOs.User
{
    public class SignUpDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class CreateUserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string CompanyId { get; set; }
    }
}
