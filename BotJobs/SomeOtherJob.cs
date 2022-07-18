using NCrontab;

namespace BotJobAndCommands.BotJobs;

public class SomeOtherJob : IBotJob
{
    public Guid ID { get; set; }
    public CrontabSchedule Schedule { get; set; }
    public Action<IBotJob> JobHasFinished { get; set; }
    public bool IsFireAndForget { get; init; } = false;

    public SomeOtherJob()
    {
        Schedule = CrontabSchedule.Parse("*/2 * * * * *", options: new() { IncludingSeconds = true });
    }

    public async ValueTask StartJobAsync()
    {
        await Task.Delay(2);
        Console.WriteLine(ID);
        Console.WriteLine("OHHHHHHHHHHHHHHHHHHHHHHHH");
        JobHasFinished(this);
    }
}
