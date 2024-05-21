namespace HeatKeeper.Server.Locations;

[RequireAdminRole]
[Post("api/locations")]
public record CreateLocationCommand(string Name, string Description) : CreateCommand;

public class CreateLocation(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateLocationCommand>
{
    public async Task HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteInsertAsync(sqlProvider.InsertLocation, command);
}






