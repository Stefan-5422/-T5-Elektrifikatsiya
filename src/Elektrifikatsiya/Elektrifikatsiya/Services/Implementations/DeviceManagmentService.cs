﻿using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;

using FluentResults;

using HiveMQtt.Client;
using HiveMQtt.Client.Results;

using Microsoft.EntityFrameworkCore;

using System.Net;
using System.Net.NetworkInformation;

namespace Elektrifikatsiya.Services.Implementations;

public class DeviceManagmentService : IDeviceManagmentService
{
	private readonly IServiceScopeFactory serviceScopeFactory;
	private readonly IDeviceStatusService deviceStatusService;
	private readonly ILogger<DeviceManagmentService> logger;
	private readonly IHiveMQClient hiveMQClient;
	private readonly HttpClient httpClient;

	public DeviceManagmentService(IServiceScopeFactory serviceScopeFactory, IDeviceStatusService deviceStatusService, ILogger<DeviceManagmentService> logger, IHiveMQClient hiveMQClient, HttpClient httpClient)
	{
		this.serviceScopeFactory = serviceScopeFactory;
		this.deviceStatusService = deviceStatusService;
		this.logger = logger;
		this.hiveMQClient = hiveMQClient;
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

	public Result<List<Device>> GetDevices(User user)
	{
		Result<List<Device>> getDevicesResult = deviceStatusService.GetDevices();

		if (getDevicesResult.IsFailed)
		{
			return getDevicesResult.ToResult();
		}

		return getDevicesResult.Value.Where(d => d.User.Id == user.Id || user.Role == Role.Admin).ToList();
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
		MainDatabaseContext mainDatabaseContext = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>();

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

		mainDatabaseContext.Entry(user).State = EntityState.Unchanged;

		_ = mainDatabaseContext.Add(device);
		Result saveDatabaseChangesResult = await Result.Try(Task () => mainDatabaseContext.SaveChangesAsync());

		if (saveDatabaseChangesResult.IsFailed)
		{
			return saveDatabaseChangesResult;
		}

		return deviceStatusService.TrackDevice(device);
	}

	public async Task<Result> UnregisterDevice(string macAdress)
	{
		using IServiceScope scope = serviceScopeFactory.CreateScope();
		MainDatabaseContext mainDatabaseContext = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>();

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

		_ = mainDatabaseContext.Remove(getDeviceResult.Value);

		return await Result.Try(Task () => mainDatabaseContext.SaveChangesAsync());
	}

	public async Task<Result> UpdateDevice(Device device)
	{
		using IServiceScope scope = serviceScopeFactory.CreateScope();
		MainDatabaseContext mainDatabaseContext = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>();

		Result<Device> getDeviceResult = GetDevice(device.MacAddress);

		if (getDeviceResult.IsFailed)
		{
			return getDeviceResult.ToResult();
		}

		Result updateDeviceResult = deviceStatusService.UpdateDeviceStatus(device);

		if (updateDeviceResult.IsFailed)
		{
			return updateDeviceResult;
		}

		PublishResult res = await hiveMQClient.PublishAsync($"shellies/shellyplug-s-{device.MacAddress}/relay/0/command", device.Enabled ? "on" : "off");

		device.User = mainDatabaseContext.Users.First(u => u.Id == device.User.Id);

		_ = mainDatabaseContext.Update(device);

		return await Result.Try(Task () => mainDatabaseContext.SaveChangesAsync());
	}
}