namespace HeatKeeper.Server.Programs;

[RequireUserRole]
[Post("api/programs/{ProgramId}/activate")]
[FromParameters]
public record ActivateProgramCommand(long ProgramId) : PostCommand;

public class ActivateProgram(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<ActivateProgramCommand>
{
    public async Task HandleAsync(ActivateProgramCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.ActivateProgram, command);
}