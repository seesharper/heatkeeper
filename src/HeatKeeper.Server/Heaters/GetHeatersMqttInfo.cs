namespace HeatKeeper.Server.Heaters;

[RequireBackgroundRole]
public record HeatersMqttInfoQuery(long ZoneId) : IQuery<HeaterMqttInfo[]>;

public record HeaterMqttInfo(string Topic, string OnPayload, string OffPayload, HeaterState HeaterState);

public class GetHeatersMqttInfo(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<HeatersMqttInfoQuery, HeaterMqttInfo[]>
{
    public async Task<HeaterMqttInfo[]> HandleAsync(HeatersMqttInfoQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<HeaterMqttInfo>(sqlProvider.GetHeatersMqttInfo, query)).ToArray();
}