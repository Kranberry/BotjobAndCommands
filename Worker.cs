namespace BotJobAndCommands;

public class Worker
{
    private Dictionary<Guid, IBotJob> ScheduledJobs = new();
    private Dictionary<Guid, IBotJob> JobsWaitingForExecution = new();

    public Guid RegisterJob(IBotJob job)
    {
        if(job.ID == default)
            job.ID = Guid.NewGuid();

        ScheduledJobs.Add(job.ID, job);
        return job.ID;
    }

    public void UnregisterJob(Guid jobId)
    {
        ScheduledJobs.Remove(jobId);
    }

    public void StartJobAtSchedule(Guid jobId)
    {
        if (JobsWaitingForExecution.ContainsKey(jobId))
            return;

        JobsWaitingForExecution.Add(jobId, ScheduledJobs[jobId]);
        _ = Task.Factory.StartNew(() => StartJob(jobId));
    }

    private async Task StartJob(Guid jobId)
    {
        IBotJob job = ScheduledJobs[jobId];

        DateTime now = DateTime.Now;
        DateTime then = job.Schedule.GetNextOccurrence(DateTime.Now);
        TimeSpan waitTime = then - now;

        Console.WriteLine($"\nJob starting at {then}");
        Console.WriteLine($"Starting job {job.ID} in {waitTime} amount of time");
        await Task.Delay(waitTime);
        await job.StartJobAsync();

        JobsWaitingForExecution.Remove(jobId);
    }

    public async Task StartWorking()
    {
        foreach (KeyValuePair<Guid, IBotJob> kvp in ScheduledJobs)
        {
            if(kvp.Value.Schedule is null)
            {
                ScheduledJobs.Remove(kvp.Key);
                Console.WriteLine($"Job with ID {kvp.Key} does not have a schedule. Removing it from jobs");
                continue;
            }

            StartJobAtSchedule(kvp.Key);
        }
    }
}