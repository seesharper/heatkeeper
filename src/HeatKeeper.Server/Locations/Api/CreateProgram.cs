namespace HeatKeeper.Server.Locations.Api;

[RequireUserRole]
[Post("api/locations/{locationId}/programs")]
public record CreateProgramCommand(string Name, string Description, long LocationId) : PostCommand;

public class CreateProgram(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateProgramCommand>
{
    public async Task HandleAsync(CreateProgramCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertProgram, command);
}
