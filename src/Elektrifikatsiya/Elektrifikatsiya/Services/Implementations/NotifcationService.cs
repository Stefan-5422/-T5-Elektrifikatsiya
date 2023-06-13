namespace Elektrifikatsiya.Services.Implementations;

public class NotifcationService : INotificationService
{
	private readonly IEmailService emailService;
	private readonly IEnergyPriceService energyPriceService;
	private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;
	private readonly ILogger<NotifcationService> logger;
	public TimeSpan ExecutionRepeatDelay => TimeSpan.FromDays(1);
	public DateTime FirstExecutionTime => DateTime.Now.AddDays(0);

	public NotifcationService(ILogger<NotifcationService> logger, IEmailService emailService, IEnergyPriceService energyPriceService, Microsoft.Extensions.Configuration.IConfiguration configuration)
	{
		this.logger = logger;
		this.emailService = emailService;
		this.energyPriceService = energyPriceService;
		this.configuration = configuration;
	}

	public async void Update()
	{
		logger.LogInformation("Sending Email");

		string url = configuration.GetValue<string>("Url") ?? "https://http.cat/images/404.jpg";

		await emailService.SendWithTemeplateAsync("stone-red@stone-red.net", "Daily Notification", "DailyNotification", templateParameters: new() { { "yesterday", $"{energyPriceService.DailyEnergyCost:F2}" }, { "yearly", $"{energyPriceService.YearlyEnergyCost:F2}" }, { "link", url } });
		logger.LogInformation("Done sending");
	}
}