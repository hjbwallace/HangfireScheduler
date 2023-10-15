using Hangfire;
using HangfireScheduler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("HangfireConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("No connection string for Hangfire configured");

await Bootstrapper.CreateDatabaseAsync(connectionString);

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString));

builder.Services.AddHangfireServer();
builder.Services.AddScheduledTasks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseHangfireDashboard("", new DashboardOptions
{
    AppPath = null,
    DashboardTitle = "Hangfire Scheduler",
    DisplayStorageConnectionString = false,
    DisplayNameFunc = DisplayNameGenerator.Generate,
});

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Services.QueueScheduledTasks();

await app.RunAsync();