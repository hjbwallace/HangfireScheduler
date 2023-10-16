namespace HangfireScheduler.Tasks
{
    public static class TaskRunner
    {
        public static Task RunAsync(IScheduledTask task)
        {
            return task.RunAsync();
        }
    }
}