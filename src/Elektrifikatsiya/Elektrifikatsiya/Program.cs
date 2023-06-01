global using INotificationService = Elektrifikatsiya.Services.INotifcationService;

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.Material;
using Blazorise.Material;

using Elektrifikatsiya.Database;
using Elektrifikatsiya.Models;
using Elektrifikatsiya.Services;
using Elektrifikatsiya.Services.Implementations;
using Elektrifikatsiya.Utilities;
using HiveMQtt.Client;
using HiveMQtt.Client.Options;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Read configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetRequiredSection("EmailSettings"));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<ServiceScheduler>();
builder.Services.AddSingleton<IUpdateService, UpdateService>();
builder.Services.AddSingleton<IDeviceStatusService, DeviceStatusService>();
builder.Services.AddSingleton<INotificationService, NotifcationService>();
builder.Services.AddSingleton<IScheduledService>(s => s.GetRequiredService<IUpdateService>());
builder.Services.AddSingleton<IScheduledService>(s => s.GetRequiredService<INotificationService>());
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddSingleton<IHiveMQClient, HiveMQClient>((provider)=>
{
	HiveMQClientOptions options = new()
	{
		Host = "localhost",
		Port = 1883,
		UseTLS = false,
	};

	HiveMQClient client = new(options);
	client.ConnectAsync().ConfigureAwait(false);
	return client;
});
builder.Services.AddTransient<IDeviceManagmentService, DeviceManagmentService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddDbContext<MainDatabaseContext>(options => options.UseSqlite("Data Source=./MainDatabase.sqlite"));
builder.Services.AddBootstrapProviders();
builder.Services.AddHttpClient<IDeviceManagmentService, DeviceManagmentService>();
builder.Services.AddBlazorise(options =>
{
	options.Immediate = true;
});

AddBlazorise(builder.Services);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	_ = app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	_ = app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();


app.Services.GetRequiredService<IOptions<EmailSettings>>().Value.CompileTemplates();

IServiceScope serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

MainDatabaseContext mainDatabase = serviceScope.ServiceProvider.GetRequiredService<MainDatabaseContext>();
mainDatabase.Database.EnsureCreated();

IAuthenticationService authenticationService = serviceScope.ServiceProvider.GetRequiredService<IAuthenticationService>();

if (!mainDatabase.Users.Any())
{
	_ = authenticationService.RegisterUserAsync("admin", "admin", Role.Admin);
}

app.Run();

static void AddBlazorise(IServiceCollection services)
{
	_ = services.AddBlazorise();
	_ = services.AddMaterialProviders();
	_ = services.AddMaterialIcons();
}