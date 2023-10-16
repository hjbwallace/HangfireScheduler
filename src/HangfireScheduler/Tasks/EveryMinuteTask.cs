namespace HangfireScheduler.Tasks
{
    [ScheduledTask(
        Schedule = "*/1 * * * *",
        Description = "Logs every minute")]
    public class EveryMinuteTask : IScheduledTask
    {
        public EveryMinuteTask()
        {
            Console.WriteLine("Building EveryMinuteTask!");
        }

        public Task RunAsync()
        {
            Console.WriteLine("Running EveryMinuteTask!");
            return Task.CompletedTask;
        }
    }
}