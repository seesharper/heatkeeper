using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Locations;

public class GetLocationDetails : IQueryHandler<GetLocationDetailsQuery, LocationDetails>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetLocationDetails(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<LocationDetails> HandleAsync(GetLocationDetailsQuery query, CancellationToken cancellationToken = default)
    {
        return (await _dbConnection.ReadAsync<LocationDetails>(_sqlProvider.GetLocationDetails, new { id = query.LocationId })).Single();
    }
}

[RequireAdminRole]
public record GetLocationDetailsQuery(long LocationId) : IQuery<LocationDetails>;

public record LocationDetails(long Id, string Name, string Description, long? DefaultOutsideZoneId, long? DefaultInsideZoneId);

