using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record UpdateSetPointCommand(long SetPointId, double Value, double Hysteresis);

public class UpdateSetPointCommandHandler : ICommandHandler<UpdateSetPointCommand>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public UpdateSetPointCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(UpdateSetPointCommand command, CancellationToken cancellationToken = default)
        => await _dbConnection.ExecuteAsync(_sqlProvider.UpdateSetPoint, command);
}