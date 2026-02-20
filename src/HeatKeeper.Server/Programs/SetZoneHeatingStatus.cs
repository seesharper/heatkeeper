using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Heaters;
using HeatKeeper.Server.Mqtt;

namespace HeatKeeper.Server.Programs;


[RequireBackgroundRole]
public record SetZoneHeatingStatusCommand(long ZoneId, HeatingStatus HeatingStatus);

public class SetZoneHeatingStatus(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor) : ICommandHandler<SetZoneHeatingStatusCommand>
{
    public async Task HandleAsync(SetZoneHeatingStatusCommand command, CancellationToken cancellationToken = default)
    {

        var heatersMqttInfo = await queryExecutor.ExecuteAsync(new HeatersMqttInfoQuery(command.ZoneId), cancellationToken);

        if (command.HeatingStatus == HeatingStatus.On)
        {
            foreach (var heaterMqttInfo in heatersMqttInfo)
            {
                // Only turn on heaters that are idle or active
                if (heaterMqttInfo.HeaterState == HeaterState.Idle || heaterMqttInfo.HeaterState == HeaterState.Active)
                {
                    await commandExecutor.ExecuteAsync(new PublishMqttMessageCommand(heaterMqttInfo.Topic, heaterMqttInfo.OnPayload), cancellationToken);
                }
                else
                {
                    // Ensure disabled heaters are turned off
                    await commandExecutor.ExecuteAsync(new PublishMqttMessageCommand(heaterMqttInfo.Topic, heaterMqttInfo.OffPayload), cancellationToken);
                }
            }
        }
        else
        {
            foreach (var heaterMqttInfo in heatersMqttInfo)
            {
                await commandExecutor.ExecuteAsync(new PublishMqttMessageCommand(heaterMqttInfo.Topic, heaterMqttInfo.OffPayload), cancellationToken);
            }
        }
    }
}

public enum HeatingStatus
{
    On,
    Off
}