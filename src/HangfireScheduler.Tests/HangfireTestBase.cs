using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HangfireScheduler.Tests
{
    public abstract class HangfireTestBase : IClassFixture<HangfireWebApplicationFactory>
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly DateTime _utcNow;

        public HangfireTestBase(HangfireWebApplicationFactory factory)
        {
            _factory = factory;

            var now = DateTime.UtcNow;
            _utcNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, now.Kind);
        }

        protected static RecurringJobDto[] GetRecurringJobs()
        {
            return JobStorage.Current
                .GetConnection()
                .GetRecurringJobs()
                .ToArray();
        }

        protected static RecurringJobDto GetRecurringJob(string id)
        {
            return GetRecurringJobs().Single(x => x.Id == id);
        }
    }
}