namespace Elektrifikatsiya.Utilities;

public static class TokenGenerator
{
    /// <summary>
    /// Creates secure token and uses an UID to make it unique.
    /// </summary>
    /// <param name="tokenType">The type of the token.</param>
    /// <param name="uid">The UID of a user.</param>
    /// <param name="length">The length of the token.</param>
    /// <returns></returns>
    public static string GenerateToken(string tokenType, int uid = 0, int length = 128)
    {
        return tokenType + "-" + SecureStringGenerator.CreateCryptographicRandomString(length, uid);
    }

    /// <summary>
    /// Checks if a token is valid.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <param name="tokenType">The type the token should have.</param>
    /// <returns></returns>
    public static bool ValidateToken(string? token, string tokenType)
    {
        if (token is null)
        {
            return false;
        }

        return token.StartsWith(tokenType + "-");
    }
}