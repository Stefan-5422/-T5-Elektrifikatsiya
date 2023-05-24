using Elektrifikatsiya.Utilities;

namespace Elektrifikatsiya.Services.Implementations;

public class ServiceScheduler : IHostedService
{
    private readonly ILogger<ServiceScheduler> logger;
    private readonly IEnumerable<IScheduledService> scheduledServices;
    private readonly Dictionary<IScheduledService, DateTime> lastExectued;

    private Timer timer = null!;

    public ServiceScheduler(ILogger<ServiceScheduler> logger, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        scheduledServices = serviceProvider.GetServices<IScheduledService>();
        lastExectued = scheduledServices.ToDictionary(x => x, x => x.FirstExecutionTime);

    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting service scheduler...");

        timer = new Timer(async (_) => await Update(), null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);

        logger.LogInformation("Service scheduler started.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping service scheduler.");

        timer.Dispose();

        logger.LogInformation("Stopped service scheduler.");

        return Task.CompletedTask;
    }

    private async Task Update()
    {
        TimeSpan s = lastExectued.Min(x => (x.Value + x.Key.ExecutionRepeatDelay)) - DateTime.Now;

        if (s < TimeSpan.Zero)
        {
            s = TimeSpan.FromSeconds(1);
        }

        timer.Change(s, Timeout.InfiniteTimeSpan);

        foreach (IScheduledService scheduledService in scheduledServices.Where(s => s.FirstExecutionTime < DateTime.Now))
        {
            if (lastExectued[scheduledService] + scheduledService.ExecutionRepeatDelay <= DateTime.Now)
            {
                lastExectued[scheduledService] = DateTime.Now;
                scheduledService.Update();
            }
        }
    }
}