namespace HeatKeeper.Server.Locations;

public class LocationExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<LocationExistsQuery, bool>
{
    public async Task<bool> HandleAsync(LocationExistsQuery query, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteScalarAsync<bool>(sqlProvider.LocationExists, query);
}

[RequireUserRole]
public record LocationExistsQuery(string Name, long Id = 0) : IQuery<bool>;
