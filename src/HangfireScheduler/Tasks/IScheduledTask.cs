namespace HangfireScheduler.Tasks
{
    public interface IScheduledTask
    {
        Task RunAsync();
    }
}