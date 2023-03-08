﻿using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;

using FluentResults;

using System.Net;
using System.Net.NetworkInformation;

namespace Elektrifikatsiya.Services.Implementations;

public class DeviceManagmentService : IDeviceManagmentService
{
	private readonly IServiceScopeFactory serviceScopeFactory;
	private readonly IDeviceStatusService deviceStatusService;
	private readonly ILogger<DeviceManagmentService> logger;
	private readonly HttpClient httpClient;

	public DeviceManagmentService(IServiceScopeFactory serviceScopeFactory, IDeviceStatusService deviceStatusService, ILogger<DeviceManagmentService> logger, HttpClient httpClient)
	{
		this.serviceScopeFactory = serviceScopeFactory;
		this.deviceStatusService = deviceStatusService;
		this.logger = logger;
		this.httpClient = httpClient;
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

	public async Task<Result<Device>> RegisterDevice(IPAddress ip, User user, string? name = null, string room = "default")
	{
		using IServiceScope scope = serviceScopeFactory.CreateScope();
		DeviceManagmentDatabaseContext deviceManagmentDatabaseContext = scope.ServiceProvider.GetRequiredService<DeviceManagmentDatabaseContext>();

		ShellyResponse? shellyResponse = null;

		try
		{
			shellyResponse = await httpClient.GetFromJsonAsync<ShellyResponse>($"http://{ip}/shelly");
		}
		catch (Exception ex)
		{
			logger.LogError("HTTP request failed! {message}", ex.Message);
		}

		if (shellyResponse is null || shellyResponse.Type != "SHPLG-S")
		{
			return Result.Fail("Device is not reachable or not a \"SHPLG-S\"!");
		}

		string mac = shellyResponse.Mac;

		if (!PhysicalAddress.TryParse(mac, out _))
		{
			return Result.Fail("Invalid mac address!");
		}

		Device device = new Device(mac, name ?? mac, ip, user, room);

		_ = deviceManagmentDatabaseContext.Add(device);
		Result saveDatabaseChangesResult = await Result.Try(Task () => deviceManagmentDatabaseContext.SaveChangesAsync());

		if (saveDatabaseChangesResult.IsFailed)
		{
			return saveDatabaseChangesResult;
		}

		return deviceStatusService.TrackDevice(device);
	}

	public async Task<Result> UnregisterDevice(string macAdress)
	{
		using IServiceScope scope = serviceScopeFactory.CreateScope();
		DeviceManagmentDatabaseContext deviceManagmentDatabaseContext = scope.ServiceProvider.GetRequiredService<DeviceManagmentDatabaseContext>();

		Result<Device> getDeviceResult = GetDevice(macAdress);

		if (getDeviceResult.IsFailed)
		{
			return getDeviceResult.ToResult();
		}

		Result untrackDeviceResult = deviceStatusService.UntrackDevice(macAdress);

		if (untrackDeviceResult.IsFailed)
		{
			return untrackDeviceResult;
		}

		_ = deviceManagmentDatabaseContext.Remove(getDeviceResult.Value);

		return await Result.Try(Task () => deviceManagmentDatabaseContext.SaveChangesAsync());
	}

	public async Task<Result> UpdateDevice(Device device)
	{
		using IServiceScope scope = serviceScopeFactory.CreateScope();
		DeviceManagmentDatabaseContext deviceManagmentDatabaseContext = scope.ServiceProvider.GetRequiredService<DeviceManagmentDatabaseContext>();

		Result result = deviceStatusService.UpdateDeviceStatus(device);

		if (result.IsFailed)
		{
			return result;
		}

		_ = deviceManagmentDatabaseContext.Update(device);

		return await Result.Try(Task () => deviceManagmentDatabaseContext.SaveChangesAsync());
	}
}