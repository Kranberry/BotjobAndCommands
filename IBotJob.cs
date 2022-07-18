using NCrontab;

namespace BotJobAndCommands;

public interface IBotJob
{
    public Guid ID { get; set; }
    public CrontabSchedule Schedule { get; set; }

    public ValueTask StartJobAsync();
}