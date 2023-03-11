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
    private readonly IMqttClientWrapper _mqttClientWrapper;
    private readonly IQueryExecutor _queryExecutor;
    private const string CommandPrefix = "cmnd/";

    public GetHeatingStatusQueryHandler(IMqttClientWrapper mqttClientWrapper, IQueryExecutor queryExecutor)
    {
        _mqttClientWrapper = mqttClientWrapper;
        _queryExecutor = queryExecutor;
    }

    public async Task<HeatingStatusResult> HandleAsync(GetHeatingStatusQuery query, CancellationToken cancellationToken = default)
    {
        var zoneMqttInfo = await _queryExecutor.ExecuteAsync(new GetZoneMqttInfoQuery(query.ZoneId), cancellationToken);

        TaskCompletionSource<HeatingStatusResult> tsc = new();
        if (!string.IsNullOrWhiteSpace(zoneMqttInfo.Topic))
        {
            await _mqttClientWrapper.Subscribe(new Subscription($"{CommandPrefix}{zoneMqttInfo.Topic}", async payload =>
            {
                if (string.Equals(payload, "OFF"))
                {
                    tsc.SetResult(HeatingStatusResult.Off);
                }
                else
                {
                    tsc.SetResult(HeatingStatusResult.On);
                }
            }));
            return await tsc.Task;
        }
        else
        {
            return HeatingStatusResult.Unknown;
        }
    }
}

public enum HeatingStatusResult
{
    Unknown,
    On,
    Off,
}