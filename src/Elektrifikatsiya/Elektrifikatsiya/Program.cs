using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Material;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapBlazorHub();
    _ = endpoints.MapFallbackToPage("/_Host");
});

app.Run();

void AddBlazorise(IServiceCollection services)
{
    _ = services
        .AddBlazorise();
    _ = services
        .AddMaterialProviders()
        .AddMaterialIcons();
}