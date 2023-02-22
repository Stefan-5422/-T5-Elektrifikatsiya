using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;

using Microsoft.EntityFrameworkCore;

namespace Elektrifikatsiya.Services.Implementations;

public class UpdateService : IHostedService, IDisposable
{
    private readonly ILogger<UpdateService> logger;
    private readonly IDeviceStatusService deviceStatusService;
    private readonly DeviceManagmentDatabaseContext deviceManagmentDatabaseContext;
    private Timer? timer = null;

    public UpdateService(ILogger<UpdateService> logger, IDeviceStatusService deviceStatusService, DeviceManagmentDatabaseContext deviceManagmentDatabaseContext)
    {
        this.logger = logger;
        this.deviceStatusService = deviceStatusService;
        this.deviceManagmentDatabaseContext = deviceManagmentDatabaseContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting update service...");

        foreach (Device device in await deviceManagmentDatabaseContext.Devices.AsNoTracking().ToListAsync(cancellationToken))
        {
            _ = deviceStatusService.TrackDevice(device);
        }

        timer = new Timer(Update, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));

        logger.LogInformation("Update service started.");
    }

    private void Update(object? state)
    {
        //TODO: Update all devices.
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