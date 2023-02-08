using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.Material;
using Blazorise.Material;

using Elektrifikatsiya.Database;

using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<UserDatabaseContext>(options => options.UseSqlite("Data Source=./UserDatabase.sqlite"));

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

IServiceScope serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
serviceScope.ServiceProvider.GetRequiredService<UserDatabaseContext>().Database.EnsureCreated();

app.Run();

void AddBlazorise(IServiceCollection services)
{
    _ = services
        .AddBlazorise();
    _ = services
        .AddMaterialProviders()
        .AddMaterialIcons();
}
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders();