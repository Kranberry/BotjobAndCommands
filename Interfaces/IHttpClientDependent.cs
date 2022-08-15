namespace BotJobAndCommands;

public interface IHttpClientDependent
{
    HttpClient HttpClient { get; }

    public void AddHttpClient(HttpClient client);
}