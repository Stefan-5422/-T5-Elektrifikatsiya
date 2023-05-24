using Elektrifikatsiya.Utilities;

namespace Elektrifikatsiya.Services.Implementations;

public class NotifcationService : INotifcationService
{
	public TimeSpan ExecutionRepeatDelay => TimeSpan.FromSeconds(30);
	public DateTime FirstExecutionTime => DateTime.Now.AddDays(0);
	private readonly IEmailService emailService;
	private readonly ILogger<NotifcationService> logger;

	public NotifcationService(ILogger<NotifcationService> logger, IEmailService emailService)
	{
		this.logger = logger;
		this.emailService = emailService;
	}

	public async void Update()
	{
		//logger.LogInformation("Sending Email");
		//await emailService.SendWithTemeplateAsync("manuel.strubegger@htl-saalfelden.at", "AAAA", "DailyNotification", templateParameters: new() { { "yesterday", "10" }, { "yearly", "100" }, { "link", "localhost:5565/yes" } });
		//logger.LogInformation("Done sending");
	}
}