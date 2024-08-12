namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record DeleteProgramCommand(long ProgramId) : DeleteCommand;

public class DeleteProgramCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteProgramCommand>
{
    public async Task HandleAsync(DeleteProgramCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteProgram, command);
}