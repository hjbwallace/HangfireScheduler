using Hangfire;
using Hangfire.Storage;
using HangfireScheduler.Tasks;
using Microsoft.Data.SqlClient;

namespace HangfireScheduler
{
    public static class Bootstrapper
    {
        public static async Task CreateDatabaseAsync(string connectionString)
        {
            using var databaseConnection = new SqlConnection(connectionString);
            using var masterConnection = new SqlConnection(connectionString + ";database=master");
            using var command = masterConnection.CreateCommand();

            command.CommandText = $@"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '{databaseConnection.Database}')
	CREATE DATABASE [{databaseConnection.Database}]";

            await masterConnection.OpenAsync();
            await command.ExecuteScalarAsync();
        }

        public static void AddScheduledTasks(this IServiceCollection services)
        {
            var baseType = typeof(ScheduledTask);
            var types = baseType.Assembly
                .GetTypes()
                .Where(baseType.IsAssignableFrom)
                .Where(x => x.IsClass && !x.IsAbstract && !x.IsInterface)
                .ToArray();

            foreach (var type in types)
                services.AddTransient(baseType, type);
        }

        public static void QueueScheduledTasks(this IServiceProvider services)
        {
            var recurringJobsManager = services.GetRequiredService<IRecurringJobManager>();
            var existingRecurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs();

            var jobOptions = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Australia Standard Time"),
            };

            foreach (var recurringJob in existingRecurringJobs)
                recurringJobsManager.RemoveIfExists(recurringJob.Id);

            var scheduledTasks = services.GetServices<ScheduledTask>() ?? Enumerable.Empty<ScheduledTask>();

            foreach (var scheduledTask in scheduledTasks)
            {
                recurringJobsManager.AddOrUpdate(
                    scheduledTask.GetType().Name,
                    () => scheduledTask.RunAsync(),
                    scheduledTask.Schedule,
                    jobOptions);
            }
        }
    }
}