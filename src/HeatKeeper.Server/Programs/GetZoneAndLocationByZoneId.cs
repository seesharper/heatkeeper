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
public record GetZoneAndLocationByZoneId(long ZoneId) : IQuery<ZoneAndLocation>;

public class GetZoneAndLocationByZoneQueryHandler : IQueryHandler<GetZoneAndLocationByZoneId, ZoneAndLocation>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetZoneAndLocationByZoneQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<ZoneAndLocation> HandleAsync(GetZoneAndLocationByZoneId query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<ZoneAndLocation>(_sqlProvider.GetZoneAndLocationByZoneId, query)).Single();
}

public record ZoneAndLocation(string ZoneName, string LocationName);