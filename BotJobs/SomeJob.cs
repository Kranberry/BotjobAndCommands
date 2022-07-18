using NCrontab;

namespace BotJobAndCommands.BotJobs;

public class SomeJob : IBotJob
{
    public Guid ID { get; set; }
    public CrontabSchedule Schedule { get; set; }

    public SomeJob()
    {
        Schedule = CrontabSchedule.Parse("*/1 * * * *");
    }

    public async ValueTask StartJobAsync()
    {
        await Task.Delay(2);
        Console.WriteLine(ID);
        Console.WriteLine("AHHHHHHHHHHHHHHHHHHHH");
    }
}
