using NCrontab;

namespace BotJobAndCommands.BotJobs;

public class GetSomeRandomThing : IBotJob, IHttpClientDependent
{
    public Guid ID { get; set; }
    public CrontabSchedule Schedule { get; set ; }

    private readonly string BaseUrl = "https://evilinsult.com/";
    public HttpClient HttpClient { get; private set; }
    public Action<IBotJob> JobHasFinished { get; set; }
    public bool IsFireAndForget { get; init; } = true;

    public void AddHttpClient(HttpClient client)
    {
        HttpClient = client;
    }

    public GetSomeRandomThing()
    {
        ID = Guid.NewGuid();
        CrontabSchedule.ParseOptions asd = new();
        asd.IncludingSeconds = true;
        Schedule = CrontabSchedule.Parse("*/27 * * * * *", asd);
    }

    public async ValueTask StartJobAsync()
    {
        string response = await GetThing();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Evil Insult: ");
        Console.WriteLine(response);
        Console.ResetColor();
    }

    private async Task<string> GetThing()
    {
        HttpResponseMessage response = await HttpClient.GetAsync(BaseUrl + "generate_insult.php?lang=en&amp;type=json");
        return await response.Content.ReadAsStringAsync();
    }
}
