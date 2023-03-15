using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Sensors;

[RequireBackgroundRole]
public record DeadSensorsQuery() : IQuery<DeadSensor[]>;

public record DeadSensor(long Id, string ExternalId, string Zone, string Location, DateTime lastSeen);

public class GetDeadSensorsQueryHandler : IQueryHandler<DeadSensorsQuery, DeadSensor[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetDeadSensorsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<DeadSensor[]> HandleAsync(DeadSensorsQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<DeadSensor>(_sqlProvider.GetDeadSensors, new { LastExpectedReading = DateTime.UtcNow.AddHours(-12) })).ToArray();
}