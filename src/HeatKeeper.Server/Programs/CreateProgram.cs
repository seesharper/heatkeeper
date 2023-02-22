using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;


[RequireUserRole]
public record CreateProgramCommand(string Name, long LocationId)
{
    public long ProgramId { get; set; }
}


public class CreateProgramCommandHandler : ICommandHandler<CreateProgramCommand>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public CreateProgramCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(CreateProgramCommand command, CancellationToken cancellationToken = default)
    {
        await _dbConnection.ExecuteAsync(_sqlProvider.InsertProgram, command);
        command.ProgramId = await _dbConnection.ExecuteScalarAsync<long>(_sqlProvider.GetLastInsertedRowId);
    }

}
