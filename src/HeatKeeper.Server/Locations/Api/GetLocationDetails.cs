namespace HeatKeeper.Server.Locations.Api;

[RequireAdminRole]
[Get("api/locations/{locationId}")]
public record GetLocationDetailsQuery(long LocationId) : IQuery<LocationDetails>;

public record LocationDetails(long Id, string Name, string Description, long? DefaultOutsideZoneId, long? DefaultInsideZoneId, long? ActiveProgramId, double? Longitude, double? Latitude, double FixedEnergyPrice, bool UseFixedEnergyPrice);

public class GetLocationDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetLocationDetailsQuery, LocationDetails>
{
    public async Task<LocationDetails> HandleAsync(GetLocationDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<LocationDetails>(sqlProvider.GetLocationDetails, new { id = query.LocationId })).Single();
}