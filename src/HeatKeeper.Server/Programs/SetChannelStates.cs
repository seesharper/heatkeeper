using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record SetChannelStatesCommand();

public class SetChannelStates : ICommandHandler<SetChannelStatesCommand>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICommandExecutor _commandExecutor;

    public SetChannelStates(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
    {
        _queryExecutor = queryExecutor;
        _commandExecutor = commandExecutor;
    }

    public async Task HandleAsync(SetChannelStatesCommand command, CancellationToken cancellationToken = default)
    {
        TargetSetPoint[] targetSetPoints = await _queryExecutor.ExecuteAsync(new GetTargetSetPointsQuery(), cancellationToken);
        MeasuredZoneTemperature[] measuredZoneTemperatures = await _queryExecutor.ExecuteAsync(new GetMeasuredTemperaturesPerZoneQuery(DateTime.UtcNow.AddMinutes(-10)), cancellationToken);

        foreach (TargetSetPoint targetSetPoint in targetSetPoints)
        {
            MeasuredZoneTemperature measuredZoneTemperature = measuredZoneTemperatures.SingleOrDefault(mzt => mzt.ZoneId == targetSetPoint.ZoneId);
            if (measuredZoneTemperature is null)
            {
                await _commandExecutor.ExecuteAsync(new SetZoneHeatingStatusCommand(targetSetPoint.ZoneId, HeatingStatus.Off), cancellationToken);
            }
            else
            {
                if (measuredZoneTemperature.Value >= targetSetPoint.Value + targetSetPoint.Hysteresis)
                {
                    await _commandExecutor.ExecuteAsync(new SetZoneHeatingStatusCommand(targetSetPoint.ZoneId, HeatingStatus.Off), cancellationToken);
                }
                if (measuredZoneTemperature.Value <= targetSetPoint.Value - targetSetPoint.Hysteresis)
                {
                    await _commandExecutor.ExecuteAsync(new SetZoneHeatingStatusCommand(targetSetPoint.ZoneId, HeatingStatus.On), cancellationToken);
                }
            }
        }
    }
}