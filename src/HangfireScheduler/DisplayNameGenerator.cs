using Hangfire.Common;
using Hangfire.Dashboard;
using HangfireScheduler.Tasks;
using System.Collections.Concurrent;
using System.Reflection;

namespace HangfireScheduler
{
    public static class DisplayNameGenerator
    {
        private static readonly ConcurrentDictionary<Type, string> DisplayNames = new();

        public static string Generate(DashboardContext context, Job job)
        {
            var type = job.Type == typeof(TaskRunner)
                ? job.Args.First().GetType()
                : job.Type;

            return DisplayNames.GetOrAdd(type, GenerateDisplay);
        }

        private static string GenerateDisplay(Type type)
        {
            return type.GetCustomAttribute<ScheduledTaskAttribute>()?.Description ?? type.Name;
        }
    }
}