namespace HangfireScheduler.Tasks
{
    public abstract class ScheduledTask
    {
        public abstract string Schedule { get; }

        public abstract Task RunAsync();
    }
}