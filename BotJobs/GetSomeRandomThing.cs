using NCrontab;

namespace BotJobAndCommands.BotJobs;

public class GetSomeRandomThing : IBotJob
{
    public Guid ID { get; set; }
    public CrontabSchedule Schedule { get; set ; }

    private readonly string BaseUrl = "https://evilinsult.com/";
    private HttpClient HttpClient;

    public GetSomeRandomThing(HttpClient client)
    {
        ID = Guid.NewGuid();
        HttpClient = client;
        HttpClient.BaseAddress = new Uri(BaseUrl);
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
        HttpResponseMessage response = await HttpClient.GetAsync("generate_insult.php?lang=en&amp;type=json");
        return await response.Content.ReadAsStringAsync();
    }
}
