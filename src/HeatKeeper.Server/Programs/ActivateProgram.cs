namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record ActivateProgramCommand(long ProgramId);

public class ActivateProgram(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<ActivateProgramCommand>
{
    public async Task HandleAsync(ActivateProgramCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.ActivateProgram, command);
}