namespace HeatKeeper.Server.Locations;

public class InsertLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<InsertLocationCommand>
{
    public async Task HandleAsync(InsertLocationCommand command, CancellationToken cancellationToken = default)
    {
        await dbConnection.ExecuteAsync(sqlProvider.InsertLocation, command);
        command.SetResult(await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetLastInsertedRowId));
    }
}

[RequireAdminRole]
public record InsertLocationCommand(string Name, string Description) : Command<long>;
