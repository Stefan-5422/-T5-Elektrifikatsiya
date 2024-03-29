﻿using Elektrifikatsiya.Models;
using FluentResults;

namespace Elektrifikatsiya.Services.Implementations;

public class DeviceStatusService : IDeviceStatusService
{
    private readonly ILogger<DeviceStatusService> logger;
    public event EventHandler<DeviceStatusChangedEventArgs>? OnDeviceStatusChanged;

    private readonly Dictionary<string, Device> devices = new();

    public DeviceStatusService(ILogger<DeviceStatusService> logger)
    {
        this.logger = logger;
	}

    public Result<List<Device>> GetDevices()
    {
        return devices.Values.ToList();
    }

    public Result UpdateDeviceStatus(Device device)
    {
        Device? modDevice = devices.GetValueOrDefault(device.MacAddress);

        if (modDevice is null)
        {
            return Result.Fail("Device not tracked!");
        }

        modDevice.PowerUsage = device.PowerUsage;
        modDevice.Enabled = device.Enabled;
        modDevice.IpAddress = device.IpAddress;
        modDevice.Name = device.Name;

        OnDeviceStatusChanged?.Invoke(this, new DeviceStatusChangedEventArgs(device.MacAddress));

        return Result.Ok();
    }

    public Result TrackDevice(Device device)
    {
        if (devices.ContainsKey(device.MacAddress))
        {
            return Result.Fail("Device already tracked!");
        }

        logger.LogInformation("Tracking new Device {device}", device.IpAddress);

        devices[device.MacAddress] = device;

        return Result.Ok();
    }

    public Result UntrackDevice(string macAddress)
    {
        return devices.Remove(macAddress) ? Result.Ok() : Result.Fail("Device is not tracked!");
    }
}