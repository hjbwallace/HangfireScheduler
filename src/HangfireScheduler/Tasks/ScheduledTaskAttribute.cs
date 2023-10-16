namespace HangfireScheduler.Tasks
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScheduledTaskAttribute : Attribute
    {
        public string? Schedule { get; set; }
        public string? Description { get; set; }
    }
}