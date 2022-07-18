using NCrontab;

namespace BotJobAndCommands.BotJobs;

public class GetBreadPun : IBotJob
{
    public Guid ID { get; set; }
    private HttpClient HttpClient { get; set; }
    public CrontabSchedule Schedule { get ; set; }

    public GetBreadPun(HttpClient client)
    {
        HttpClient = client;
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
    }

    private async Task<string> GetThing()
    {
        HttpResponseMessage response = await HttpClient.GetAsync("https://my-bao-server.herokuapp.com/api/breadpuns");
        return await response.Content.ReadAsStringAsync();
    }
}
