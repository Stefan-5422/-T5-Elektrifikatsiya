namespace Elektrifikatsiya.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string html, string? from = null);
    
    Task SendWithTemeplateAsync(string to, string subject, string templateKey, string? from = null, Dictionary<string, string>? templateParameters = null);
}