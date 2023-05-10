using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;
using Elektrifikatsiya.Utilities;

using FluentResults;

using Microsoft.EntityFrameworkCore;

namespace Elektrifikatsiya.Services.Implementations;

public class UpdateService : IHostedService, IDisposable
{
    private readonly ILogger<UpdateService> logger;
    private readonly IDeviceStatusService deviceStatusService;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private Timer? timer = null;

    public UpdateService(ILogger<UpdateService> logger, IDeviceStatusService deviceStatusService, IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.deviceStatusService = deviceStatusService;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        IServiceScope serviceScope = serviceScopeFactory.CreateScope();
        MainDatabaseContext mainDatabaseContext = serviceScope.ServiceProvider.GetRequiredService<MainDatabaseContext>();

        logger.LogInformation("Starting update service...");

        foreach (Device device in await mainDatabaseContext.Devices.Include(d => d.User).AsNoTracking().ToListAsync(cancellationToken))
        {
            _ = deviceStatusService.TrackDevice(device);
        }

        timer = new Timer(async (_) => await Update(), null, TimeSpan.Zero, TimeSpan.FromSeconds(15));

        logger.LogInformation("Update service started.");
    }

    private async Task Update()
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping update service.");

        _ = timer?.Change(Timeout.Infinite, 0);

        logger.LogInformation("Stopped update service.");

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        timer?.Dispose();
    }
}