namespace IdentityServer.Helpers;

public class PasswordHelper
{
    public static string HashPassword(string password)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA256())
        {
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(salt.Concat(hash).ToArray());
        }
    }
    public static bool VerifyPassword(string hashedPassword, string password)
    {
        var parts = Convert.FromBase64String(hashedPassword);
        var salt = parts.Take(32).ToArray();
        var hash = parts.Skip(32).ToArray();
        using (var hmac = new System.Security.Cryptography.HMACSHA256(salt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }
    }
}
