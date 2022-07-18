using static PublicReadonlyProperties;

namespace BotJobAndCommands.Commands;

public class ExitCommand : ICommand
{
    public string Command { get; init; } = "Exit";
    public string? ShortCommand { get; init; } = "e";
    public string Description { get; init; } = "Exit and close the application";

    public async ValueTask RunCommand(string[] parameters)
    {
        SetManuealResetEvent();
    }

    private void SetManuealResetEvent() => MyManualResetEvent.Set();
}
