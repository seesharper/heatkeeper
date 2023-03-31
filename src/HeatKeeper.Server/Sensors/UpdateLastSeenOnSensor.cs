using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Sensors;

[RequireBackgroundRole]
public record UpdateLastSeenOnSensorCommand(string ExternalId, DateTime LastSeen);

public class UpdateLastSeenOnSensorCommandHandler : ICommandHandler<UpdateLastSeenOnSensorCommand>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public UpdateLastSeenOnSensorCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(UpdateLastSeenOnSensorCommand command, CancellationToken cancellationToken = default)
        => await _dbConnection.ExecuteAsync(_sqlProvider.UpdateLastSeenOnSensor, command);
}