using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record AddAllSchedulesToJanitorCommand();

public class AddAllSchedulesToJanitorCommandHandler : ICommandHandler<AddAllSchedulesToJanitorCommand>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;

    public AddAllSchedulesToJanitorCommandHandler(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
    {
        _queryExecutor = queryExecutor;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(AddAllSchedulesToJanitorCommand command, CancellationToken cancellationToken = default)
    {
        GetAllSchedules.Result[] allSchedules = await _queryExecutor.ExecuteAsync(new GetAllSchedulesQuery());
        foreach (GetAllSchedules.Result schedule in allSchedules)
        {
            await _commandExecutor.ExecuteAsync(new AddScheduleToJanitorCommand(schedule.Id, schedule.Name, schedule.CronExpression), cancellationToken);
        }
    }
}