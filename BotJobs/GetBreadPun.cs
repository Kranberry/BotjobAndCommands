using NCrontab;

namespace BotJobAndCommands.BotJobs;

public class GetBreadPun : IBotJob, IHttpClientDependent
{
    public Guid ID { get; set; }
    public HttpClient HttpClient { get; private set; }
    public CrontabSchedule Schedule { get ; init; }
    public Action<IBotJob> JobHasFinished { get; set; }
    public bool IsFireAndForget { get; init; } = false;

    public void AddHttpClient(HttpClient client)
    {
        HttpClient = client;
    }

    public GetBreadPun()
    {
        CrontabSchedule.ParseOptions asd = new();
        asd.IncludingSeconds = true;
        Schedule = CrontabSchedule.Parse("0 */1 * * * *", asd);
    }

    public async ValueTask StartJobAsync()
    {
        string response = await GetThing();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(response);
        Console.ResetColor();
        JobHasFinished(this);
    }

    private async Task<string> GetThing()
    {
        HttpResponseMessage response = await HttpClient.GetAsync("https://my-bao-server.herokuapp.com/api/breadpuns");
        return await response.Content.ReadAsStringAsync();
    }
}
