using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record ClearActiveProgramCommand(long ProgramId);

public class ClearActiveProgram : ICommandHandler<ClearActiveProgramCommand>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public ClearActiveProgram(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(ClearActiveProgramCommand command, CancellationToken cancellationToken = default)
        => await _dbConnection.ExecuteAsync(_sqlProvider.ClearActiveProgram, command);
}