using Microsoft.Extensions.Configuration;

namespace BotJobAndCommands;

public class AppConfig
{
    private static IConfiguration _Configuration = new ConfigurationBuilder().AddJsonFile($"AppSecrets.json", true).AddEnvironmentVariables().Build();

    private static string GetValueOfKey(string key) => _Configuration[key];

    internal static string DiscordBotToken => GetValueOfKey("DiscordBot:Token");
}