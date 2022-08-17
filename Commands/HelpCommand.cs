using static PublicReadonlyProperties;

namespace BotJobAndCommands.Commands;

public class HelpCommand : ICommand
{
    public string Command { get; init; } = "Help";
    public string? ShortCommand { get; init; }
    public string Description { get; init; } = "Shows every command and their description";
    public bool IsBotCommand { get; init; } = false;

    public async ValueTask<CommandResponse> RunCommand(string[] parameters)
    {
        string specificHelpCommand = parameters.Length > 1 ? parameters[1] : string.Empty;

        if (string.IsNullOrEmpty(specificHelpCommand))
            ShowHelpPrompt();
        else
            ShowHelpOfSpecificCommand(specificHelpCommand);
        return new CommandResponse(Command, "Available commands listed", null);
    }

    private void ShowHelpOfSpecificCommand(string command)
    {
        ICommand? theCommand = AvailableCommands.Values.FirstOrDefault(c => c.Command.ToLower() == command || c.ShortCommand?.ToLower() == command);
        if (theCommand is null)
            return;

        Console.WriteLine("------------------------------------------HELP--------------------------------------");
        Console.WriteLine("[Command] : [ShortCommand] | [Description]\n");

        Console.WriteLine($"{theCommand.Command} : {theCommand.ShortCommand} | {theCommand.Description}");

        Console.ResetColor();
        Console.WriteLine("------------------------------------------------------------------------------------");
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
        Console.ResetColor();
        Console.WriteLine("------------------------------------------------------------------------------------");
    }
}