using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Programs;


[RequireBackgroundRole]
public record SetZoneHeatingStatusCommand(long ZoneId, HeatingStatus HeatingStatus);

public class SetZoneHeatingStatus : ICommandHandler<SetZoneHeatingStatusCommand>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;
    public SetZoneHeatingStatus(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
    {
        _queryExecutor = queryExecutor;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(SetZoneHeatingStatusCommand command, CancellationToken cancellationToken = default)
    {
        ZoneMqttInfo zoneMqttInfo = await _queryExecutor.ExecuteAsync(new GetZoneMqttInfoQuery(command.ZoneId), cancellationToken);

        if (command.HeatingStatus == HeatingStatus.On)
        {
            await _commandExecutor.ExecuteAsync(new TasmotaCommand(zoneMqttInfo.Topic, zoneMqttInfo.OnPayload), cancellationToken);
        }
        else
        {
            await _commandExecutor.ExecuteAsync(new TasmotaCommand(zoneMqttInfo.Topic, zoneMqttInfo.OffPayload), cancellationToken);
        }
    }
}

public enum HeatingStatus
{
    On,
    Off
}