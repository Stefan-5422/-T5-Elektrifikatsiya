using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;

using FluentResults;

using System.Net;

namespace Elektrifikatsiya.Services.Implementations;

public class DeviceManagmentService : IDeviceManagmentService
{
    private readonly DeviceManagmentDatabaseContext deviceManagmentDatabaseContext;
    private readonly IDeviceStatusService deviceStatusService;

    public DeviceManagmentService(DeviceManagmentDatabaseContext deviceManagmentDatabaseContext, IDeviceStatusService deviceStatusService)
    {
        this.deviceManagmentDatabaseContext = deviceManagmentDatabaseContext;
        this.deviceStatusService = deviceStatusService;
    }

    public Result<Device> GetDevice(string macAdress)
    {
        Result<List<Device>> getDevicesResult = deviceStatusService.GetDevices();

        if (getDevicesResult.IsFailed)
        {
            return getDevicesResult.ToResult();
        }

        Device? device = getDevicesResult.Value.FirstOrDefault(d => d.MacAddress == macAdress);

        if (device is null)
        {
            return Result.Fail("Device does not exist!");
        }

        return device;
    }

    public Result<List<Device>> GetDevices()
    {
        Result<List<Device>> getDevicesResult = deviceStatusService.GetDevices();

        if (getDevicesResult.IsFailed)
        {
            return getDevicesResult.ToResult();
        }

        return getDevicesResult.Value.ToList();
    }

    public Result<List<Device>> GetDevicesInRoom(string room)
    {
        Result<List<Device>> getDevicesResult = deviceStatusService.GetDevices();

        if (getDevicesResult.IsFailed)
        {
            return getDevicesResult.ToResult();
        }

        return getDevicesResult.Value.Where(d => d.Room == room).ToList();
    }

    public Result<List<Device>> GetDevicesOfUser(int userId)
    {
        Result<List<Device>> getDevicesResult = deviceStatusService.GetDevices();

        if (getDevicesResult.IsFailed)
        {
            return getDevicesResult.ToResult();
        }

        return getDevicesResult.Value.Where(d => d.User.Id == userId).ToList();
    }

    public Task<Result<Device>> Register(IPAddress ip, User user, string? name = null, string room = "default")
    {
        throw new NotImplementedException();
    }

    public Task<Result> UnRegister(string macAdress)
    {
        throw new NotImplementedException();
    }
}