using System.Text.Json;
using System.Text.Json.Serialization;

namespace BotJobAndCommands.Commands;

public class GetChuckNorrisJokeCommand : ICommand, IHttpClientDependent
{
    public string Command { get; init; } = "ChuckNorrisJokeGet";
    public string? ShortCommand { get; init; } = "CNJG";
    public string Description { get; init; } = "Get a chuck norris joke via REST";
    public HttpClient HttpClient { get; private set; }
    public bool IsBotCommand { get; init; } = true;

    public void AddHttpClient(HttpClient client)
    {
        HttpClient = client;
    }

    public async ValueTask<CommandResponse> RunCommand(string[] parameters)
    {
        if(HttpClient is null)
        {
            throw new Exception("HttpClient is not set");
        }

        ChuckNorrisJoke cnj = await GetChuckNorrisJoke();
        Console.WriteLine(cnj.Value);
        return new CommandResponse(Command, "Chuck norris joke got", new(cnj.Value));
    }

    private async ValueTask<ChuckNorrisJoke> GetChuckNorrisJoke()
    {
        HttpResponseMessage response = await HttpClient.GetAsync("https://api.chucknorris.io/jokes/random");
        string responseAsJson = await response.Content.ReadAsStringAsync();
        ChuckNorrisJoke cnj = JsonSerializer.Deserialize<ChuckNorrisJoke>(responseAsJson)!;

        return cnj;
    }

    private record ChuckNorrisJoke([property: JsonPropertyName("value")] string Value);
}