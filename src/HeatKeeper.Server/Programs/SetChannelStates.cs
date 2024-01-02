using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record SetChannelStatesCommand();

public class SetChannelStates : ICommandHandler<SetChannelStatesCommand>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;
    private readonly ILogger<SetChannelStates> _logger;

    public SetChannelStates(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, ILogger<SetChannelStates> logger)
    {
        _queryExecutor = queryExecutor;
        _commandExecutor = commandExecutor;
        _logger = logger;
    }

    public async Task HandleAsync(SetChannelStatesCommand command, CancellationToken cancellationToken = default)
    {
        TargetSetPoint[] targetSetPoints = await _queryExecutor.ExecuteAsync(new GetTargetSetPointsQuery(), cancellationToken);
        MeasuredZoneTemperature[] measuredZoneTemperatures = await _queryExecutor.ExecuteAsync(new GetMeasuredTemperaturesPerZoneQuery(DateTime.UtcNow.AddMinutes(-10)), cancellationToken);

        foreach (TargetSetPoint targetSetPoint in targetSetPoints)
        {
            _logger.LogInformation("The target set point for zone {ZoneId} is {Value} and we have an hysteresis set to {Hysteresis}", targetSetPoint.ZoneId, targetSetPoint.Value, targetSetPoint.Hysteresis);

            MeasuredZoneTemperature measuredZoneTemperature = measuredZoneTemperatures.SingleOrDefault(mzt => mzt.ZoneId == targetSetPoint.ZoneId);
            if (measuredZoneTemperature is null)
            {
                _logger.LogWarning("We could not get the measured temperature for zone {ZoneId}. Make sure that we don't have a dead sensor. We are turning the channel off", targetSetPoint.ZoneId);
                await _commandExecutor.ExecuteAsync(new SetZoneHeatingStatusCommand(targetSetPoint.ZoneId, HeatingStatus.Off), cancellationToken);
            }
            else
            {
                if (measuredZoneTemperature.Value >= targetSetPoint.Value + targetSetPoint.Hysteresis)
                {
                    _logger.LogInformation("The measured value was {MeasuredValue} and the target setpoint is {TargetSetPoint}. We are turning the channel off.", measuredZoneTemperature.Value, targetSetPoint.Value);                     
                    await _commandExecutor.ExecuteAsync(new SetZoneHeatingStatusCommand(targetSetPoint.ZoneId, HeatingStatus.Off), cancellationToken);
                }
                if (measuredZoneTemperature.Value <= targetSetPoint.Value - targetSetPoint.Hysteresis)
                {
                    _logger.LogInformation("The measured value was {MeasuredValue} and the target setpoint is {TargetSetPoint}. We are turning the channel on.", measuredZoneTemperature.Value, targetSetPoint.Value);                      
                    await _commandExecutor.ExecuteAsync(new SetZoneHeatingStatusCommand(targetSetPoint.ZoneId, HeatingStatus.On), cancellationToken);
                }
            }
        }
    }
}