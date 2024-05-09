using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server.Locations;

[RequireAdminRole]
[Post("api/locations")]
public record CreateLocationCommand(string Name, string Description) : Command<Results<Created<ResourceId>, ProblemHttpResult>>;

public class CreateLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, ICommandExecutor commandExecutor) : ICommandHandler<CreateLocationCommand>
{
    public async Task HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var insertLocationCommand = new InsertLocationCommand(command.Name, command.Description);
        await commandExecutor.ExecuteAsync(insertLocationCommand, cancellationToken);
        var locationId = insertLocationCommand.GetResult();
        // await dbConnection.ExecuteAsync(sqlProvider.InsertLocation, command);
        // var locationId = await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetLastInsertedRowId);
        command.SetResult(TypedResults.Created("api/locations/", new ResourceId(locationId)));
    }
}






