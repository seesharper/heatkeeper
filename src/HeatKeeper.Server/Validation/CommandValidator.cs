namespace HeatKeeper.Server.Validation;

public class CommandValidator<TCommand>(IEnumerable<IValidator<TCommand>> validators, ICommandHandler<TCommand> handler) : ICommandHandler<TCommand>
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var validator in validators)
        {
            await validator.Validate(command);
        }
        await handler.HandleAsync(command, cancellationToken);
    }
}