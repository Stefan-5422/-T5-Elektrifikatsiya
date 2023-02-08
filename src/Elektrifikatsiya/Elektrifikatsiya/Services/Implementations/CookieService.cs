using Microsoft.JSInterop;

namespace Elektrifikatsiya.Services.Implementations;

public class CookieService : ICookieService
{
    private readonly IJSRuntime jsRuntime;

    public CookieService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task WriteCookieAsync(string name, string value, int days)
    {
        _ = await jsRuntime.InvokeAsync<string>("blazorExtensions.WriteCookie", name, value, days);
    }
}