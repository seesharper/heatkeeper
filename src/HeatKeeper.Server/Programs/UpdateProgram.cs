namespace HeatKeeper.Server.Programs;

[RequireUserRole]
[Patch("api/programs/{ProgramId}")]
public record UpdateProgramCommand(long ProgramId, string Name, string Description, long? ActiveScheduleId) : PatchCommand;

public class UpdateProgramCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateProgramCommand>
{
    private readonly IDbConnection _dbConnection = dbConnection;
    private readonly ISqlProvider _sqlProvider = sqlProvider;

    public async Task HandleAsync(UpdateProgramCommand command, CancellationToken cancellationToken = default)
        => await _dbConnection.ExecuteAsync(_sqlProvider.UpdateProgram, command);
}