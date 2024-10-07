namespace HeatKeeper.Server.Validation;

public class CommandValidator<TCommand>(IEnumerable<IValidator<TCommand>> validators, ICommandHandler<TCommand> handler) : ICommandHandler<TCommand> where TCommand : IProblemCommand
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var validator in validators)
        {
            await validator.Validate(command);
        }
        if (command.HasProblemResult)
        {
            return;
        }
        await handler.HandleAsync(command, cancellationToken);
    }
}