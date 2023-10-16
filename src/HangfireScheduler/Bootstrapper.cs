using Hangfire;
using Hangfire.Storage;
using HangfireScheduler.Tasks;
using Microsoft.Data.SqlClient;
using System.Reflection;

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
            var baseType = typeof(IScheduledTask);
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

            var scheduledTasks = services.GetServices<IScheduledTask>() ?? Enumerable.Empty<IScheduledTask>();
            var scheduledTaskDetails = scheduledTasks.Select(x => new
            {
                Task = x,
                Id = x.GetType().Name,
                Details = x.GetType().GetCustomAttribute<ScheduledTaskAttribute>(),
            });

            var invalidTasks = scheduledTaskDetails
                .Where(x => x.Details == null)
                .Select(x => x.Id)
                .ToArray();

            if (invalidTasks.Any())
                throw new InvalidOperationException("Tasks are missing the `ScheduledTaskAttribute`: " + string.Join(", ", invalidTasks));

            foreach (var scheduledTask in scheduledTaskDetails)
            {
                recurringJobsManager.AddOrUpdate(
                    scheduledTask.Id,
                    () => TaskRunner.RunAsync(scheduledTask.Task),
                    scheduledTask.Details!.Schedule,
                    jobOptions);
            }
        }
    }
}