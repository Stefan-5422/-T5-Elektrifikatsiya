using Elektrifikatsiya.Models;

using FluentResults;

namespace Elektrifikatsiya.Services.Implementations;

public class AuthenticationService : IAuthenticationService
{
    public Task<Result> DeleteUserAsync(string deletionToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> GetUserAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> IsAuthenticated()
    {
        throw new NotImplementedException();
    }

    public Task<Result> LoginUserAsync(string name, string password)
    {
        throw new NotImplementedException();
    }

    public Task<Result> LogoutUserAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result> RegisterUserAsync(string name, string password)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UserExistsAsync(string name)
    {
        throw new NotImplementedException();
    }
}