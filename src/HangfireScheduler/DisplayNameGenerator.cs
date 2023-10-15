using Hangfire.Common;
using Hangfire.Dashboard;
using System.Collections.Concurrent;

namespace HangfireScheduler
{
    public static class DisplayNameGenerator
    {
        private static readonly ConcurrentDictionary<Type, string> DisplayNames = new();

        public static string Generate(DashboardContext context, Job job)
        {
            return DisplayNames.GetOrAdd(job.Type, GenerateDisplay);
        }

        private static string GenerateDisplay(Type type)
        {
            return type.Name;
        }
    }
}