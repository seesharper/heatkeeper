using System.ComponentModel.DataAnnotations;
namespace HeatKeeper.Server.Locations;

public class LocationTemperaturesQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider, IUserContext userContext) : IQueryHandler<LocationTemperaturesQuery, LocationTemperature[]>
{
    public async Task<LocationTemperature[]> HandleAsync(LocationTemperaturesQuery query, CancellationToken cancellationToken = default) 
        => (await dbConnection.ReadAsync<LocationTemperature>(sqlProvider.GetDashboardTemperatures, query)).ToArray();
}

[RequireUserRole]
public record LocationTemperaturesQuery(long LocationId ) : IQuery<LocationTemperature[]>;
      
public record LocationTemperature(
    [property: Key] long ZoneId,
    string Name,
    double? Temperature,
    double? Humidity,
    DateTime Updated
    );