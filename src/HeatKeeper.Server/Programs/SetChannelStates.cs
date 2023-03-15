using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record SetChannelStatesCommand();

public class SetChannelStates : ICommandHandler<SetChannelStatesCommand>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;
    private readonly Logger _logger;

    public SetChannelStates(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, Logger logger)
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
            _logger.Info($"The target set point for zone {targetSetPoint.ZoneId} is {targetSetPoint.Value} and we have an hysteresis set to {targetSetPoint.Hysteresis}");

            MeasuredZoneTemperature measuredZoneTemperature = measuredZoneTemperatures.SingleOrDefault(mzt => mzt.ZoneId == targetSetPoint.ZoneId);
            if (measuredZoneTemperature is null)
            {
                _logger.Warning($"We could not get the measured temperature for zone {targetSetPoint.ZoneId}. Make sure that we don't have a dead sensor. We are turning the channel off");
                await _commandExecutor.ExecuteAsync(new SetZoneHeatingStatusCommand(targetSetPoint.ZoneId, HeatingStatus.Off), cancellationToken);
            }
            else
            {
                if (measuredZoneTemperature.Value >= targetSetPoint.Value + targetSetPoint.Hysteresis)
                {
                    _logger.Info($"The measured value was {measuredZoneTemperature.Value} and the target setpoint is {targetSetPoint.Value}. We are turning the channel off.");
                    await _commandExecutor.ExecuteAsync(new SetZoneHeatingStatusCommand(targetSetPoint.ZoneId, HeatingStatus.Off), cancellationToken);
                }
                if (measuredZoneTemperature.Value <= targetSetPoint.Value - targetSetPoint.Hysteresis)
                {
                    _logger.Info($"The measured value was {measuredZoneTemperature.Value} and the target setpoint is {targetSetPoint.Value}. We are turning the channel on.");
                    await _commandExecutor.ExecuteAsync(new SetZoneHeatingStatusCommand(targetSetPoint.ZoneId, HeatingStatus.On), cancellationToken);
                }
            }
        }
    }
}