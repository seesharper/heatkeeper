using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

//NOTE
// We might need to set a max age on the measurements, so wer don't act on old data.

[RequireBackgroundRole]
public record GetMeasuredTemperaturesPerZoneQuery(DateTime SinceUtcDateTime) : IQuery<MeasuredZoneTemperature[]>;

public record MeasuredZoneTemperature(long ZoneId, double Value, DateTime Updated);

public class GetMeasuredTemperaturesPerZone : IQueryHandler<GetMeasuredTemperaturesPerZoneQuery, MeasuredZoneTemperature[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetMeasuredTemperaturesPerZone(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<MeasuredZoneTemperature[]> HandleAsync(GetMeasuredTemperaturesPerZoneQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<MeasuredZoneTemperature>(_sqlProvider.GetMeasuredTemperatureValuePerZone, query)).ToArray();
}