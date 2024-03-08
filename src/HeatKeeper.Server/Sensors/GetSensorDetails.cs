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

[RequireAdminRole]
public record SensorDetailsQuery(long SensorId) : IQuery<SensorDetails>;

public record SensorDetails(long Id, string Name, string Description, string ExternalId, DateTime LastSeen, string ZoneName);

public class GetSensorDetailsQueryHandle(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<SensorDetailsQuery, SensorDetails>
{
    public async Task<SensorDetails> HandleAsync(SensorDetailsQuery query, CancellationToken cancellationToken = default)
    {
        return (await dbConnection.ReadAsync<SensorDetails>(sqlProvider.GetSensorDetails, query)).Single();
    }
}