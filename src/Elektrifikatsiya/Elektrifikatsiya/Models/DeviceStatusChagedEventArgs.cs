namespace Elektrifikatsiya.Models;

public class DeviceStatusChagedEventArgs : EventArgs
{
    public string MacAddress { get; set; }

    public DeviceStatusChagedEventArgs(string macAddress)
    {
        MacAddress = macAddress;
    }
}