using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record ActivateProgramCommand(long ProgramId);

public class ActivateProgram : ICommandHandler<ActivateProgramCommand>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public ActivateProgram(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(ActivateProgramCommand command, CancellationToken cancellationToken = default)
        => await _dbConnection.ExecuteAsync(_sqlProvider.ActivateProgram, command);
}