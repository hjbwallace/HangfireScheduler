namespace HangfireScheduler.Tasks
{
    public class EveryMinuteTask : ScheduledTask
    {
        public EveryMinuteTask()
        {
            Console.WriteLine("Building EveryMinuteTask!");
        }

        public override string Schedule => "*/1 * * * *";

        public override Task RunAsync()
        {
            Console.WriteLine("Running EveryMinuteTask!");
            return Task.CompletedTask;
        }
    }
}