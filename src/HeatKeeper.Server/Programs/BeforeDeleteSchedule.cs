using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;

namespace HeatKeeper.Server.Programs;

public class BeforeDeleteSchedule : ICommandHandler<DeleteScheduleCommand>
{
    private readonly ICommandHandler<DeleteScheduleCommand> _handler;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;

    public BeforeDeleteSchedule(ICommandHandler<DeleteScheduleCommand> handler, IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
    {
        _handler = handler;
        _queryExecutor = queryExecutor;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(DeleteScheduleCommand command, CancellationToken cancellationToken = default)
    {
        SetPointInfo[] setPoints = await _queryExecutor.ExecuteAsync(new SetPointsByScheduleQuery(command.ScheduleId), cancellationToken);
        foreach (SetPointInfo setPoint in setPoints)
        {
            await _commandExecutor.ExecuteAsync(new DeleteSetPointCommand(setPoint.Id), cancellationToken);
        }

        await _commandExecutor.ExecuteAsync(new SetActiveScheduleIdToNullCommand(command.ScheduleId), cancellationToken);

        await _handler.HandleAsync(command, cancellationToken);
    }
}