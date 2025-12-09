namespace HeatKeeper.Server.Lights.Api;

[RequireUserRole]
[Get("api/lights/{lightId}")]
public record LightDetailsQuery(long LightId) : IQuery<LightDetails>;

public record LightDetails(long Id, string Name, string ZoneName, string Description, string MqttTopic, string OnPayload, string OffPayload, bool Enabled);

public class GetLightDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<LightDetailsQuery, LightDetails>
{
    public async Task<LightDetails> HandleAsync(LightDetailsQuery query, CancellationToken cancellationToken = default) =>
        (await dbConnection.ReadAsync<LightDetails>(sqlProvider.GetLightDetails, query)).Single();
}
