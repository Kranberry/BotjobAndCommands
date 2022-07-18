using NCrontab;

namespace BotJobAndCommands;

public interface IBotJob
{
    public Guid ID { get; set; }
    public CrontabSchedule Schedule { get; set; }
    public Action<IBotJob> JobHasFinished { get; set; }
    public bool IsFireAndForget { get; init; }

    public ValueTask StartJobAsync();
}