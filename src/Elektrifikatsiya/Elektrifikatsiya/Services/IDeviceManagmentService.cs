using Elektrifikatsiya.Models;

using FluentResults;

using System.Net;

namespace Elektrifikatsiya.Services;

public interface IDeviceManagmentService
{
    public Task<Result<Device>> RegisterDevice(IPAddress ip, User user, string? name = null, string room = "default");

    public Task<Result> UnregisterDevice(string macAdress);

    public Result<Device> GetDevice(string macAdress);

    public Result<List<Device>> GetDevices();

    public Result<List<Device>> GetDevicesInRoom(string room);

    public Result<List<Device>> GetDevicesOfUser(int userId);

    public Task<Result> UpdateDevice(Device device);
}