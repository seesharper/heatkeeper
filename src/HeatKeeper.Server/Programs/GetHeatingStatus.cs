using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Mqtt;

namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record GetHeatingStatusQuery(long ZoneId) : IQuery<HeatingStatusResult>;

public class GetHeatingStatusQueryHandler : IQueryHandler<GetHeatingStatusQuery, HeatingStatusResult>
{
    private readonly ITasmotaClient _tasmotaClient;
    private readonly IQueryExecutor _queryExecutor;

    public GetHeatingStatusQueryHandler(ITasmotaClient tasmotaClient, IQueryExecutor queryExecutor)
    {
        _tasmotaClient = tasmotaClient;
        _queryExecutor = queryExecutor;
    }

    public async Task<HeatingStatusResult> HandleAsync(GetHeatingStatusQuery query, CancellationToken cancellationToken = default)
    {
        ZoneMqttInfo zoneMqttInfo = await _queryExecutor.ExecuteAsync(new GetZoneMqttInfoQuery(query.ZoneId), cancellationToken);

        if (!string.IsNullOrWhiteSpace(zoneMqttInfo.Topic))
        {
            string payload = await _tasmotaClient.GetStatus(zoneMqttInfo.Topic);

            if (string.Equals(payload, "OFF", StringComparison.InvariantCultureIgnoreCase))
            {
                return HeatingStatusResult.Off;
            }
            else
            {
                return HeatingStatusResult.On;
            }
        };

        return HeatingStatusResult.Unknown;
    }
}

public enum HeatingStatusResult
{
    Unknown,
    On,
    Off,
}