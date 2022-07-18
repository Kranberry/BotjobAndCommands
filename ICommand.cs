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
    /// Run the command
    /// </summary>
    /// <param name="parameters">The parameters for said command</param>
    public ValueTask RunCommand(string[] parameters);
}