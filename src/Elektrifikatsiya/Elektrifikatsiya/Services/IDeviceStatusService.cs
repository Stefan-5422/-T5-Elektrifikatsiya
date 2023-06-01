using Elektrifikatsiya.Models;

using FluentResults;

namespace Elektrifikatsiya.Services;

public interface IDeviceStatusService
{
    public event EventHandler<DeviceStatusChangedEventArgs> OnDeviceStatusChanged;

    public Result<List<Device>> GetDevices();

    public Result UpdateDeviceStatus(Device device);

    public Result TrackDevice(Device device);

    public Result UntrackDevice(string macAddress);
}