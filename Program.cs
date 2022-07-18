using BotJobAndCommands;
using BotJobAndCommands.BotJobs;
using System.Reflection;
using static PublicReadonlyProperties;

HttpClient httpClient = new();
Worker worker = new();
//worker.RegisterJob(new GetSomeRandomThing(client));
//worker.RegisterJob(new GetBreadPun(client));
worker.RegisterJob(new SomeJob());
worker.RegisterJob(new SomeOtherJob());

SetupCommands<ICommand>(httpClient);

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

#region Block the application to close until MyManualResetEvent is set

MyManualResetEvent.WaitOne();

#endregion

#region Cleanup possible resources

httpClient.Dispose();
Console.ResetColor();

#endregion



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