namespace Order.Application.Common.Interfaces;

public interface IIdentityGrpcClient
{
    /// <summary>
    /// Gets the full name of the user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The full name of the user.</returns>
    Task<string> GetUserFullNameAsync(string userId);
    /// <summary>
    /// Gets the email of the user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The email of the user.</returns>
    Task<string> GetUserEmailAsync(string userId);
}
