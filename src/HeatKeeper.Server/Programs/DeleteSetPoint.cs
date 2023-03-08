using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record DeleteSetPointCommand(long SetPointId);

public class DeleteSetPointCommandHandler : ICommandHandler<DeleteSetPointCommand>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public DeleteSetPointCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(DeleteSetPointCommand command, CancellationToken cancellationToken = default)
        => await _dbConnection.ExecuteAsync(_sqlProvider.DeleteSetPoint, command);
}