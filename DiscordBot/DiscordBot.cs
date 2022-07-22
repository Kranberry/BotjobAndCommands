using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Text.Json;

namespace BotJobAndCommands.DiscordBot;

public class DiscordBot
{
    private string BotSecret { get; init; }
    private DiscordClient _client;
    private Dictionary<string, ICommand> _availableCommands;

    public DiscordBot(Dictionary<string, ICommand> availableCommands)
    {
        _availableCommands = availableCommands;
        BotSecret = AppConfig.DiscordBotToken;
    }
    
    public async Task MainAsync()
    {
        DiscordConfiguration config = new() { Token = BotSecret, TokenType = TokenType.Bot, Intents = DiscordIntents.AllUnprivileged };
        _client = new(config);

        _client.MessageCreated += MessageResponser;

        await _client.ConnectAsync();
        await Task.Delay(-1);
    }

    private async Task MessageResponser(DiscordClient client, MessageCreateEventArgs messageEvent)
    {
        string message = messageEvent.Message.Content;
        if (message[0] is not '!')
            return;

        message = message.Substring(1);
        if (_availableCommands.ContainsKey(message.ToLower()))
        {
            ICommand command = _availableCommands[message.ToLower()];
            if (!command.IsBotCommand)
                return;

            CommandResponse response = await command.RunCommand(Array.Empty<string>());
            await messageEvent.Message.RespondAsync(response.BotResponse!.ResponseMessage);
        }
    }
}