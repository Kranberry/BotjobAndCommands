using NCrontab;

namespace BotJobAndCommands.BotJobs;

public class SomeOtherJob : IBotJob
{
    public Guid ID { get; set; }
    public CrontabSchedule Schedule { get; set; }

    public SomeOtherJob()
    {
        Schedule = CrontabSchedule.Parse("*/2 * * * *");
    }

    public async ValueTask StartJobAsync()
    {
        await Task.Delay(2);
        Console.WriteLine(ID);
        Console.WriteLine("OHHHHHHHHHHHHHHHHHHHHHHHH");
    }
}
