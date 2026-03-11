using HeatKeeper.Server.Events;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.SmartMeter;

[RequireBackgroundRole]
public record PublishSmartMeterReadingsCommand;

public class PublishSmartMeterReadings(IDbConnection dbConnection, ISqlProvider sqlProvider, IEventBus eventBus, ILogger<PublishSmartMeterReadings> logger) : ICommandHandler<PublishSmartMeterReadingsCommand>
{
    public async Task HandleAsync(PublishSmartMeterReadingsCommand command, CancellationToken cancellationToken = default)
    {
        var readings = (await dbConnection.ReadAsync<SmartMeterReadings>(cancellationToken, sqlProvider.GetSmartMeterReadings)).Single();
        logger.LogInformation("Publishing smart meter readings: {@Readings}", readings);
        await eventBus.PublishAsync(readings, cancellationToken);
    }
}
