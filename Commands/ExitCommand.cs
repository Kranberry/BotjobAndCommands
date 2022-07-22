using static PublicReadonlyProperties;

namespace BotJobAndCommands.Commands;

public class ExitCommand : ICommand
{
    public string Command { get; init; } = "Exit";
    public string? ShortCommand { get; init; } = "e";
    public string Description { get; init; } = "Exit and close the application";
    public bool IsBotCommand { get; init; } = false;

    public async ValueTask<CommandResponse> RunCommand(string[] parameters)
    {
        SetManuealResetEvent();
        return new CommandResponse(Command, "Shutting down", null);
    }

    private void SetManuealResetEvent() => MyManualResetEvent.Set();
}
