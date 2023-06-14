using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;

namespace Elektrifikatsiya.Services.Implementations;

public class NotifcationService : INotificationService
{
    public TimeSpan ExecutionRepeatDelay => TimeSpan.FromDays(1);
    public DateTime FirstExecutionTime => DateTime.Now.AddDays(0);
    private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;
    private readonly IEmailService emailService;
    private readonly IEnergyPriceService energyPriceService;
    private readonly ILogger<NotifcationService> logger;
    private readonly MainDatabaseContext mainDatabaseContext;

    public NotifcationService(ILogger<NotifcationService> logger, IEmailService emailService, IEnergyPriceService energyPriceService, Microsoft.Extensions.Configuration.IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.emailService = emailService;
        this.energyPriceService = energyPriceService;
        this.configuration = configuration;

        mainDatabaseContext = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<MainDatabaseContext>();
    }

    public async void Update()
    {
        logger.LogInformation("Sending Email");

        string url = configuration.GetValue<string>("Url") ?? "https://http.cat/images/404.jpg";

        await foreach (User user in mainDatabaseContext.Users)
        {
            await emailService.SendWithTemeplateAsync(user.Email, "Daily Notification", "DailyNotification", templateParameters: new() { { "yesterday", $"{energyPriceService.DailyEnergyCost:F2}" }, { "yearly", $"{energyPriceService.YearlyEnergyCost:F2}" }, { "link", url } });
        }

        logger.LogInformation("Done sending");
    }
}