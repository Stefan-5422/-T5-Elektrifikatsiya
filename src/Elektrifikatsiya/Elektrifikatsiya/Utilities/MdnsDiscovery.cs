using Elektrifikatsiya.Models;

using Microsoft.Extensions.Caching.Memory;

using System.Net;

using Tmds.MDns;

namespace Elektrifikatsiya.Utilities;

public static class MdnsDiscovery
{
    public static event Action<IPAddress> OnDeviceFound = null!;

    private static readonly ServiceBrowser serviceBrowser = new ServiceBrowser();
    private static readonly HttpClient client = new();
    private static readonly MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

    static MdnsDiscovery()
    {
        serviceBrowser.StartBrowse("_http._tcp");
        serviceBrowser.ServiceAdded += async (_, eventArgs) => await AddDevice(eventArgs.Announcement);
        serviceBrowser.ServiceChanged += async (_, eventArgs) => await AddDevice(eventArgs.Announcement);
        serviceBrowser.ServiceRemoved += async (_, eventArgs) => await AddDevice(eventArgs.Announcement);
    }

    public static void FetchChachedDevices()
    {
        serviceBrowser.Services.ToList().ForEach(async d => await AddDevice(d));
    }

    private static async Task AddDevice(ServiceAnnouncement announcement)
    {
        IPAddress ipAddress = announcement.Addresses.First();

        if (!memoryCache.TryGetValue(ipAddress.ToString(), out bool isShellyDevice))
        {
            isShellyDevice = await IsShellyDevice(ipAddress);
            _ = memoryCache.Set(ipAddress.ToString(), isShellyDevice, TimeSpan.FromHours(1));
        }

        if (isShellyDevice)
        {
            OnDeviceFound?.Invoke(ipAddress);
        }
    }

    private static async Task<bool> IsShellyDevice(IPAddress ipAddress)
    {
        ShellyResponse? shellyResponse;

        try
        {
            shellyResponse = await client.GetFromJsonAsync<ShellyResponse>($"http://{ipAddress}/shelly");
        }
        catch (HttpRequestException)
        {
            return false;
        }

        if (shellyResponse is null || shellyResponse.Type != "SHPLG-S")
        {
            return false;
        }

        return true;
    }
}