using HangfireScheduler.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireScheduler.Tests
{
    public class HangfireTests : HangfireTestBase
    {
        public HangfireTests(HangfireWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public void ScheduledTasksAreRegistered()
        {
            var tasks = _factory.Services.GetServices<IScheduledTask>();
            Assert.Equal(2, tasks.Count());
        }

        [Fact]
        public void RecurringTasksAreAvailable()
        {
            var tasks = _factory.Services.GetServices<IScheduledTask>();
            var recurringJobs = GetRecurringJobs();

            Assert.Equal(tasks.Count(), recurringJobs.Length);
        }

        [Fact]
        public void LastExecutionTimeIsEmpty()
        {
            var recurringJobs = GetRecurringJobs();
            Assert.All(recurringJobs, x => Assert.Null(x.LastExecution));
        }

        [Fact]
        public void EveryMinuteTaskExecutionIsQueued()
        {
            var everyMinuteTask = GetRecurringJob("EveryMinuteTask");
            var expectedExecution = _utcNow.AddMinutes(1);

            Assert.Equal(expectedExecution, everyMinuteTask.NextExecution.GetValueOrDefault());
        }

        [Fact]
        public void EveryTwoMinutesTaskExecutionIsQueued()
        {
            var everyTwoMinutesTask = GetRecurringJob("EveryTwoMinutesTask");
            var expectedExecution = _utcNow.AddMinutes(_utcNow.Minute % 2 == 0 ? 2 : 1);

            Assert.Equal(expectedExecution, everyTwoMinutesTask.NextExecution.GetValueOrDefault());
        }
    }
}