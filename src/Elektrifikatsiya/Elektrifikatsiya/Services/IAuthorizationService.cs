using Elektrifikatsiya.Models;

using FluentResults;

namespace Elektrifikatsiya.Services;

public interface IAuthorizationService
{
    /// <summary>
    /// Check if the current user is authorized.
    /// </summary>
    /// <param name="requiredRole">The minimum required role.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="bool"/> that indicates if the user is authorized.</returns>
    Task<Result<bool>> IsAuthorized(Role requiredRole);
}