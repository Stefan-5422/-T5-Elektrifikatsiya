using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
		IServiceScope serviceScope =	    serviceScopeFactory.CreateScope();
		DeviceManagmentDatabaseContext deviceManagmentDatabaseContext = serviceScope.ServiceProvider.GetRequiredService<DeviceManagmentDatabaseContext>();

		logger.LogInformation("Starting update service...");

        foreach (Device device in await deviceManagmentDatabaseContext.Devices.AsNoTracking().ToListAsync(cancellationToken))
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

        foreach(Device device in getDeviceStatusResult.Value)
        {
            //TODO: Some update magic
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