using Elektrifikatsiya.Models;

using FluentResults;

namespace Elektrifikatsiya.Services;

public interface IAuthenticationService
{
    /// <summary>
    /// Gets the current user.
    /// </summary>
    /// <returns> <see cref="Task"/> to <see langword="await"/> and the currently authenticated user.</returns>
    Task<Result<User>> GetUserAsync();

    Task<Result<List<User>>> GetUsers();

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="name">The name address of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="role">The default role of the user.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>

    Task<Result> RegisterUserAsync(string name, string password, string email, Role role);

    /// <summary>
    /// Logs a user in.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task<Result> LoginUserAsync(string name, string password);

    /// <summary>
    /// Logs the user out.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task<Result> LogoutUserAsync();

    /// <summary>
    /// Deletes the current user.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task<Result> DeleteUserAsync();

	Task<Result> DeleteUser(User user);

	/// <summary>
	/// Checks if a user exists.
	/// </summary>
	/// <param name="name">The name address of the user.</param>
	/// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the user exists.</returns>
	Task<Result<bool>> UserExistsAsync(string name);

    /// <summary>
    /// Check if the current user is authenticated.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the user is authenticated.</returns>
    Task<Result<bool>> IsAuthenticated();
}