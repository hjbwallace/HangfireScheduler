namespace HangfireScheduler.Tasks
{
    public class EveryMinuteTask
    {
        public EveryMinuteTask()
        {
            Console.WriteLine("Building EveryMinuteTask!");
        }

        public void Run()
        {
            Console.WriteLine("Running EveryMinuteTask!");
        }
    }
}
