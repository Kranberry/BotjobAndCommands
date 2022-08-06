using HtmlAgilityPack;

namespace BotJobAndCommands.Commands;
internal class ApartmentScrapeCommand : ICommand, IHttpClientDependent
{
    public HttpClient HttpClient { get; private set; }

    public string Command { get; init; } = "ApartmentsGet";
    public string? ShortCommand { get; init; } = "AptGet";
    public string Description { get; init; }
    public bool IsBotCommand { get; init; } = true;

    public void AddHttpClient(HttpClient client)
    {
        HttpClient = client;
    }

    public async ValueTask<CommandResponse> RunCommand(string[] parameters)
    {
        await ScrapeWillhelm();
        return new CommandResponse(Command, "AHHH", new("AHHHHH"));
    }

    private async Task ScrapeWillhelm()
    {
        //string response = await HttpClient.GetStringAsync("https://www.willhem.se/sok-bostad/Malmo/");
        string response = await HttpClient.GetStringAsync("https://www.willhem.se/sok-bostad/Jonkoping/");

        HtmlDocument doc = new();
        doc.LoadHtml(response);

        IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants("table");

        foreach(HtmlNode node in nodes)
            Console.WriteLine(node.InnerHtml);
    }
}