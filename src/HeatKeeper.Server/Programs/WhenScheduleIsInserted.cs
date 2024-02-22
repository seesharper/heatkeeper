using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;

namespace HeatKeeper.Server.Programs;

public class WhenScheduleIsCreated : ICommandHandler<CreateScheduleCommand>
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ICommandHandler<CreateScheduleCommand> _CreateScheduleCommandHandler;

    public WhenScheduleIsCreated(ICommandExecutor commandExecutor, ICommandHandler<CreateScheduleCommand> CreateScheduleCommandHandler)
    {
        _commandExecutor = commandExecutor;
        _CreateScheduleCommandHandler = CreateScheduleCommandHandler;
    }

    public async Task HandleAsync(CreateScheduleCommand command, CancellationToken cancellationToken = default)
    {
        await _CreateScheduleCommandHandler.HandleAsync(command, cancellationToken);
        await _commandExecutor.ExecuteAsync(new AddScheduleToJanitorCommand(command.ScheduleId, command.Name, command.CronExpression), cancellationToken);
    }
}
