using Elektrifikatsiya.Models;

using FluentResults;

using System.Net;

namespace Elektrifikatsiya.Services;

public interface IDeviceManagmentService
{
    public Task<Result<Device>> Register(IPAddress ip, User user, string? name = null, string room = "default");

    public Task<Result> Unregister(string macAdress);

    public Result<Device> GetDevice(string macAdress);

    public Result<List<Device>> GetDevices();

    public Result<List<Device>> GetDevicesInRoom(string room);

    public Result<List<Device>> GetDevicesOfUser(int userId);
}