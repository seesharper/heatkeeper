using HeatKeeper.Server.Chargers;

namespace HeatKeeper.Server.Chargers.Api;

[RequireUserRole]
[Get("api/chargers/{chargerId}")]
public record ChargerDetailsQuery(long ChargerId) : IQuery<ChargerDetails>;

public record ChargerDetails(long Id, string Name, string ZoneName, string Description, string MqttTopic, string OnPayload, string OffPayload, ChargerState ChargerState);

public class GetChargerDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<ChargerDetailsQuery, ChargerDetails>
{
    public async Task<ChargerDetails> HandleAsync(ChargerDetailsQuery query, CancellationToken cancellationToken = default) =>
        (await dbConnection.ReadAsync<ChargerDetails>(sqlProvider.GetChargerDetails, query)).Single();
}
