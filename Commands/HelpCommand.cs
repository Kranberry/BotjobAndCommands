using static PublicReadonlyProperties;

namespace BotJobAndCommands.Commands;

public class HelpCommand : ICommand
{
    public string Command { get; init; } = "Help";
    public string? ShortCommand { get; init; }
    public string Description { get; init; } = "Shows every command and their description";

    public async ValueTask RunCommand(string[] parameters)
    {
        ShowHelpPrompt();
    }

    private void ShowHelpPrompt()
    {
        List<string> commandsWrittenOut = new();
        Console.WriteLine("------------------------------------------HELP--------------------------------------");
        Console.WriteLine("[Command] : [ShortCommand] | [Description]\n");
        int index = 1;
        foreach (KeyValuePair<string, ICommand> kvp in AvailableCommands)
        {
            if (commandsWrittenOut.Contains(kvp.Value.Command))
                continue;

            if (index % 2 == 0)
                Console.ForegroundColor = ConsoleColor.DarkGray;
            else
                Console.ResetColor();

            string addedShortCommand = kvp.Value.ShortCommand is not null or "" ? $" : {kvp.Value.ShortCommand}" : string.Empty;
            Console.WriteLine($"{kvp.Value.Command}{addedShortCommand} | {kvp.Value.Description}");
            commandsWrittenOut.Add(kvp.Value.Command);
            index++;
        }
        Console.WriteLine("------------------------------------------------------------------------------------");
    }
}
