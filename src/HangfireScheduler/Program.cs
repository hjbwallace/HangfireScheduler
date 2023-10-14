using Hangfire;
using HangfireScheduler;
using HangfireScheduler.Tasks;

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

builder.Services.AddTransient<EveryMinuteTask>();

var app = builder.Build();
var backgroundJobs = app.Services.GetRequiredService<IBackgroundJobClient>();
var recurringJobs = app.Services.GetRequiredService<IRecurringJobManager>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseHangfireDashboard();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

var task = app.Services.GetRequiredService<EveryMinuteTask>();
recurringJobs.AddOrUpdate("Job", () => task.Run(), "*/1 * * * *");

await app.RunAsync();