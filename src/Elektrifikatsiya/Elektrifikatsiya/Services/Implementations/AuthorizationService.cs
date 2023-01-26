using Elektrifikatsiya.Models;

using FluentResults;

namespace Elektrifikatsiya.Services.Implementations;

public class AuthorizationService : IAuthorizationService
{
    private readonly IAuthenticationService authenticationService;

    public AuthorizationService(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    public async Task<Result<bool>> IsAuthorized(Role requiredRole)
    {
        Result<User> userResult = await authenticationService.GetUserAsync();

        if (userResult.IsSuccess && userResult.Value.Role <= requiredRole)
        {
            return Result.Ok(true);
        }
        else
        {
            return Result.Fail("Use doesn't have the required role!");
        }
    }
}