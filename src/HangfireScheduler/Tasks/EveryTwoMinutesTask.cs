namespace HangfireScheduler.Tasks
{
    public class EveryTwoMinutesTask : ScheduledTask
    {
        public EveryTwoMinutesTask()
        {
            Console.WriteLine("Building EveryTwoMinutesTask!");
        }

        public override string Schedule => "*/2 * * * *";

        public override Task RunAsync()
        {
            Console.WriteLine("Running EveryTwoMinutesTask!");
            return Task.CompletedTask;
        }
    }
}