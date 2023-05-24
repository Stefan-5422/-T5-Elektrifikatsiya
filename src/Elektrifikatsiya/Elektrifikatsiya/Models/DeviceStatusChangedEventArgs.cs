namespace Elektrifikatsiya.Models;

public class DeviceStatusChangedEventArgs : EventArgs
{
    public string MacAddress { get; set; }

    public DeviceStatusChangedEventArgs(string macAddress)
    {
        MacAddress = macAddress;
    }
}