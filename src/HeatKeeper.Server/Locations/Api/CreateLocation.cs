namespace HeatKeeper.Server.Locations;

[RequireAdminRole]
[Post("api/locations")]
public record CreateLocationCommand(string Name, string Description) : PostCommand;

public class CreateLocation(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateLocationCommand>
{
    public async Task HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertLocation, command);
}






