namespace HeatKeeper.Server.Locations;

public class WhenLocationIsInserted(ICommandHandler<InsertLocationCommand> handler, ICommandExecutor commandExecutor, IUserContext userContext) : ICommandHandler<InsertLocationCommand>
{
    public async Task HandleAsync(InsertLocationCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        await AddCurrentUserToThisLocation(command.GetResult(), cancellationToken);
    }

    private async Task AddCurrentUserToThisLocation(long locationId, CancellationToken cancellationToken)
    {
        var addUserToLocationCommand = new AddUserToLocationCommand(userContext.Id, locationId);
        await commandExecutor.ExecuteAsync(addUserToLocationCommand, cancellationToken);
    }
}