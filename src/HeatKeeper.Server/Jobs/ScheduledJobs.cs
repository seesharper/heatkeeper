using Janitor;

namespace HeatKeeper.Server.Jobs;

[RequireUserRole]
[Get("api/jobs")]
public record GetScheduledJobsQuery : IQuery<ScheduledJob[]>;

public record ScheduledJob(string Name, string State, DateTime? NextOccurrence);

public class GetScheduledJobs(IJanitor janitor) : IQueryHandler<GetScheduledJobsQuery, ScheduledJob[]>
{
    public Task<ScheduledJob[]> HandleAsync(GetScheduledJobsQuery query, CancellationToken cancellationToken = default)
    {
        var test = janitor.First();
        
        var jobs = janitor
            .Select(task => new ScheduledJob(task.Name, task.State.ToString(), task.NextOccurrence))
            .ToArray();

        return Task.FromResult(jobs);
    }
}
