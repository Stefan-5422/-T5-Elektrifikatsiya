using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;
using Elektrifikatsiya.Utilities;

using FluentResults;

using Microsoft.EntityFrameworkCore;

namespace Elektrifikatsiya.Services.Implementations;

public class UpdateService : IUpdateService
{
    private readonly ILogger<UpdateService> logger;
    private readonly IDeviceStatusService deviceStatusService;

    public UpdateService(ILogger<UpdateService> logger, IDeviceStatusService deviceStatusService, IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.deviceStatusService = deviceStatusService;

        IServiceScope serviceScope = serviceScopeFactory.CreateScope();
        MainDatabaseContext mainDatabaseContext = serviceScope.ServiceProvider.GetRequiredService<MainDatabaseContext>();

        foreach (Device device in mainDatabaseContext.Devices.Include(d => d.User).AsNoTracking().ToList())
        {
            _ = deviceStatusService.TrackDevice(device);
        }
    }

    public DateTime FirstExecutionTime { get;  } = DateTime.Now.AddSeconds(10);
    public TimeSpan ExecutionRepeatDelay { get;  } = TimeSpan.FromSeconds(1);

    public async void Update()
    {
        Result<List<Device>> getDeviceStatusResult = deviceStatusService.GetDevices();

        if (getDeviceStatusResult.IsFailed)
        {
            logger.LogError("Updating devices failed!");
        }
        PrometheusQuery promQueryer = new PrometheusQuery("http://localhost:9090");
        foreach (Device device in getDeviceStatusResult.Value)
        {
            PrometheusDataWrapper? devicePowerData = (await promQueryer.Query($"power{{sensor=\"shellyplug-s-{device.MacAddress}/relay/0\"}}"))?.Data;
            PrometheusDataWrapper? deviceStatusData = (await promQueryer.Query($"state{{sensor=\"shellyplug-s-{device.MacAddress}/relay\"}}"))?.Data;

            if (devicePowerData is not null && deviceStatusData is not null)
            {
                device.PowerUsage = devicePowerData.VectorTypeToTimestampFloatTuple()?.ValueOrDefault.Item2 ?? 0;
                device.Enabled = (deviceStatusData.VectorTypeToTimestampFloatTuple()?.ValueOrDefault.Item2 ?? 0) == 1;
                _ = deviceStatusService.UpdateDeviceStatus(device);
            }
        }
    }
}