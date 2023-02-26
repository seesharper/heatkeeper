using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using Cronos;
using HeatKeeper.Server.Validation;

namespace HeatKeeper.Server.Programs;

public class AfterScheduleHasBeenInsertedOrUpdated<TCommand> : ICommandHandler<TCommand> where TCommand : IScheduleCommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly ICommandExecutor _commandExecutor;

    public AfterScheduleHasBeenInsertedOrUpdated(ICommandHandler<TCommand> handler, ICommandExecutor commandExecutor)
    {
        _handler = handler;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        await _handler.HandleAsync(command, cancellationToken);
        await _commandExecutor.ExecuteAsync(new AddAllSchedulesToJanitorCommand(), cancellationToken);
    }
}