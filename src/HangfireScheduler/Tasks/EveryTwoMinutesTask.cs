namespace HangfireScheduler.Tasks
{
    [ScheduledTask(
        Schedule = "*/2 * * * *",
        Description = "Logs every even minute")]
    public class EveryTwoMinutesTask : IScheduledTask
    {
        public EveryTwoMinutesTask()
        {
            Console.WriteLine("Building EveryTwoMinutesTask!");
        }

        public Task RunAsync()
        {
            Console.WriteLine("Running EveryTwoMinutesTask!");
            return Task.CompletedTask;
        }
    }
}