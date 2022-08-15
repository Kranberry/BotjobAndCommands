namespace BotJobAndCommands;

public interface ICommand
{
    /// <summary>
    /// The command to be written in the console
    /// </summary>
    public string Command { get; init; }

    /// <summary>
    /// A shortcommand of the command. May not always be available.
    /// </summary>
    public string? ShortCommand { get; init; }

    /// <summary>
    /// The description of the command, describing what the command does, and how to use it.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// If true, this command should be allowed to be used by something like a discord bot.
    /// </summary>
    public bool IsBotCommand { get; init; }

    /// <summary>
    /// Run the command
    /// </summary>
    /// <param name="parameters">The parameters for said command</param>
    public ValueTask<CommandResponse> RunCommand(string[] parameters);
}

public record CommandResponse(string Command, string Message, DiscordBotReponse? BotResponse);

public record DiscordBotReponse(string ResponseMessage);