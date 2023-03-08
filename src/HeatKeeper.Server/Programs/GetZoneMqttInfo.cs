using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record GetZoneMqttInfoQuery(long ZoneId) : IQuery<ZoneMqttInfo>;

public record ZoneMqttInfo(string Topic, string OnPayload, string OffPayload);

public class GetZoneMqttInfoQueryHandler : IQueryHandler<GetZoneMqttInfoQuery, ZoneMqttInfo>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetZoneMqttInfoQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<ZoneMqttInfo> HandleAsync(GetZoneMqttInfoQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<ZoneMqttInfo>(_sqlProvider.GetZoneMqttInfo, query)).Single();
}