using Elektrifikatsiya.Models;

namespace Elektrifikatsiya.Services;

public interface IAuthenticationService
{
    /// <summary>
    /// Gets the current user.
    /// </summary>
    /// <returns> <see cref="Task"/> to <see langword="await"/> and the currently authenticated user.</returns>
    Task<User?> GetUserAsync();

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="email">The E-mail address of the user.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>

    Task RegisterUserAsync(string name, string password);

    /// <summary>
    /// Logs a user in.
    /// </summary>
    /// <param name="name">The E-mail address of the user.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task<(bool Success, string? Message)> LoginUserAsync(string name, string password);

    /// <summary>
    /// Logs the user out.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task LogoutUserAsync();

    /// <summary>
    /// Deletes the current user.
    /// </summary>
    /// <param name="deletionToken">The deletion token.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the operation was successful.</returns>
    Task<bool> DeleteUserAsync(string deletionToken);

    /// <summary>
    /// Checks if a user exists.
    /// </summary>
    /// <param name="name">The E-mail address of the user.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the user exists.</returns>
    Task<bool> UserExistsAsync(string name);

    /// <summary>
    /// Check if the current user is authenticated.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the user is authenticated.</returns>
    Task<bool> IsAuthenticated();
}