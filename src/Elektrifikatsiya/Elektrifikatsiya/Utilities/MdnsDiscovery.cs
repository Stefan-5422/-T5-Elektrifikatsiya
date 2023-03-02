using System.Net;
using FluentResults;
using Tmds.MDns;

namespace Elektrifikatsiya.Utilities;

public static class MdnsDiscovery
{
	public static event Action<IPAddress> OnDeviceFound;

	static ServiceBrowser serviceBrowser = new ServiceBrowser();

	static MdnsDiscovery()
	{
		serviceBrowser.StartBrowse("_http._tcp");
        serviceBrowser.ServiceAdded += (_, eventArgs) => OnDeviceFound?.Invoke(eventArgs.Announcement.Addresses.First());
        serviceBrowser.ServiceChanged += (_, eventArgs) => OnDeviceFound?.Invoke(eventArgs.Announcement.Addresses.First());
        serviceBrowser.ServiceRemoved += (_, eventArgs) => OnDeviceFound?.Invoke(eventArgs.Announcement.Addresses.First());
    }
}