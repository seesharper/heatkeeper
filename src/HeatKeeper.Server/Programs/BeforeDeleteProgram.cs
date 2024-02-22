using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;

namespace HeatKeeper.Server.Programs;

public class BeforeDeleteProgram : ICommandHandler<DeleteProgramCommand>
{
    private readonly ICommandHandler<DeleteProgramCommand> _handler;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;

    public BeforeDeleteProgram(ICommandHandler<DeleteProgramCommand> handler, IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
    {
        _handler = handler;
        _queryExecutor = queryExecutor;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(DeleteProgramCommand command, CancellationToken cancellationToken = default)
    {
        ScheduleInfo[] schedules = await _queryExecutor.ExecuteAsync(new SchedulesByProgramQuery(command.ProgramId), cancellationToken);
        foreach (ScheduleInfo schedule in schedules)
        {
            await _commandExecutor.ExecuteAsync(new DeleteScheduleCommand(schedule.Id), cancellationToken);
        }

        await _commandExecutor.ExecuteAsync(new ClearActiveProgramCommand(command.ProgramId), cancellationToken);

        await _handler.HandleAsync(command, cancellationToken);
    }
}