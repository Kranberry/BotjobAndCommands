using BotJobAndCommands;
using BotJobAndCommands.BotJobs;
using BotJobAndCommands.DiscordBot;
using BotJobAndCommands.SocketServer;
using NCrontab;
using System.Reflection;
using static PublicReadonlyProperties;

HttpClient httpClient = new();
Worker worker = new();
DiscordBot discordBot = new(AvailableCommands);
WebSocketServer socketServer = new(AvailableCommands);

//RegisterJobs<IBotJob>(worker, httpClient);
SetupCommands<ICommand>(httpClient);
Task.Run(socketServer.Start);

Task consoleReadTask = new(async () =>
{
    Console.WriteLine("Type 'help' for a list of commands");
    while (true)
    {
        string command = Console.ReadLine()!.ToLower().Trim();

        bool commandExists = AvailableCommands.ContainsKey(command);
        if (commandExists)
        {
            string[] arguments = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            await AvailableCommands[command].RunCommand(arguments);
        }
    }
});
consoleReadTask.Start();

_ = discordBot.MainAsync();

#region Block the application to close until MyManualResetEvent is set

MyManualResetEvent.WaitOne();

#endregion

#region Cleanup possible resources

httpClient.Dispose();
Console.ResetColor();

#endregion


void RestartRecurringJob(IBotJob job)
{
    CrontabSchedule schedule = job.Schedule;

    DateTime nextTime = schedule.GetNextOccurrence(DateTime.UtcNow);
    TimeSpan timeSpawn = nextTime - DateTime.UtcNow;

    Task.Delay(timeSpawn).ContinueWith(t => job.StartJobAsync());
}

void RegisterJobs<T>(Worker worker, HttpClient client) where T : IBotJob
{
    IEnumerable<Type> jobTypes = GetAllTypesThatImplementsInterface<T>();

    foreach (Type type in jobTypes)
    {
        T instance = (T)Activator.CreateInstance(type)!;
        worker.RegisterJob(instance);

        if (instance is IHttpClientDependent dependent)
        {
            dependent.AddHttpClient(client);
        }

        if (!instance.IsFireAndForget)
        {
            instance.JobHasFinished += RestartRecurringJob;
        }

        instance.StartJobAsync();
        Console.WriteLine("Registered job");
    }
}

void SetupCommands<T>(HttpClient httpClient) where T : ICommand
{
    IEnumerable<Type> commandTypes = GetAllTypesThatImplementsInterface<T>();

    foreach(Type type in commandTypes)
    {
        T instance = (T)Activator.CreateInstance(type)!;
        AvailableCommands.Add(instance.Command.ToLower(), instance);

        if(instance is IHttpClientDependent dependent)
        {
            dependent.AddHttpClient(httpClient);
        }

        if(instance.ShortCommand is not null)
            AvailableCommands.Add(instance.ShortCommand.ToLower(), instance);
    }
}

IEnumerable<Type> GetAllTypesThatImplementsInterface<T>()
{
    return Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
}

public static class PublicReadonlyProperties
{
    public static Dictionary<string, ICommand> AvailableCommands = new();
    public static ManualResetEvent MyManualResetEvent = new(false);
}