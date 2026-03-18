using HeatKeeper.Server.Heaters;
using HeatKeeper.Server.Mqtt;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record SetChannelStatesCommand();

public class SetChannelStates(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, ILogger<SetChannelStates> logger) : ICommandHandler<SetChannelStatesCommand>
{
    public async Task HandleAsync(SetChannelStatesCommand command, CancellationToken cancellationToken = default)
    {
        TargetSetPoint[] targetSetPoints = await queryExecutor.ExecuteAsync(new GetTargetSetPointsQuery(), cancellationToken);
        MeasuredZoneTemperature[] measuredZoneTemperatures = await queryExecutor.ExecuteAsync(new GetMeasuredTemperaturesPerZoneQuery(DateTime.UtcNow.AddMinutes(-10)), cancellationToken);

        foreach (TargetSetPoint targetSetPoint in targetSetPoints)
        {
            logger.LogInformation("The target set point for zone {ZoneId} is {Value} and we have an hysteresis set to {Hysteresis}", targetSetPoint.ZoneId, targetSetPoint.Value, targetSetPoint.Hysteresis);

            HeaterMqttInfo[] heatersMqttInfo = await queryExecutor.ExecuteAsync(new HeatersMqttInfoQuery(targetSetPoint.ZoneId), cancellationToken);
            MeasuredZoneTemperature measuredZoneTemperature = measuredZoneTemperatures.SingleOrDefault(mzt => mzt.ZoneId == targetSetPoint.ZoneId);

            if (measuredZoneTemperature is null)
            {
                logger.LogWarning("We could not get the measured temperature for zone {ZoneId}. Make sure that we don't have a dead sensor. We are turning the channel off", targetSetPoint.ZoneId);
            }
            else if (measuredZoneTemperature.Value >= targetSetPoint.Value + targetSetPoint.Hysteresis)
            {
                logger.LogInformation("The measured value was {MeasuredValue} and the target setpoint is {TargetSetPoint}. We are turning the channel off.", measuredZoneTemperature.Value, targetSetPoint.Value);
            }
            else if (measuredZoneTemperature.Value <= targetSetPoint.Value - targetSetPoint.Hysteresis)
            {
                logger.LogInformation("The measured value was {MeasuredValue} and the target setpoint is {TargetSetPoint}. We are turning the channel on.", measuredZoneTemperature.Value, targetSetPoint.Value);
            }

            foreach (HeaterMqttInfo heater in heatersMqttInfo)
            {
                if (heater.HeaterState is HeaterState.Paused or HeaterState.Disabled)
                {
                    await commandExecutor.ExecuteAsync(new PublishMqttMessageCommand(heater.Topic, heater.OffPayload), cancellationToken);
                    continue;
                }

                if (measuredZoneTemperature is null || measuredZoneTemperature.Value >= targetSetPoint.Value + targetSetPoint.Hysteresis)
                {
                    await commandExecutor.ExecuteAsync(new PublishMqttMessageCommand(heater.Topic, heater.OffPayload), cancellationToken);
                    if (heater.HeaterState == HeaterState.Active)
                    {
                        await commandExecutor.ExecuteAsync(new SetHeaterStateCommand(heater.HeaterId, HeaterState.Idle), cancellationToken);
                    }
                }
                else if (measuredZoneTemperature.Value <= targetSetPoint.Value - targetSetPoint.Hysteresis)
                {
                    await commandExecutor.ExecuteAsync(new PublishMqttMessageCommand(heater.Topic, heater.OnPayload), cancellationToken);
                    if (heater.HeaterState == HeaterState.Idle)
                    {
                        await commandExecutor.ExecuteAsync(new SetHeaterStateCommand(heater.HeaterId, HeaterState.Active), cancellationToken);
                    }
                }
                // Temperature is within the hysteresis band: leave Idle/Active heaters as-is
            }
        }
    }
}
