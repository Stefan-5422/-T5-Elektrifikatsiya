namespace Elektrifikatsiya.Services;

public interface ICookieService
{
    /// <summary>
    /// Writes a cookie.
    /// </summary>
    /// <param name="name">The name of the cookie.</param>
    /// <param name="value">The value of the cookie.</param>
    /// <param name="days">Indicates the maximum lifetime of the cookie.</param>
    /// <returns></returns>
    Task WriteCookieAsync(string name, string value, int days);
}