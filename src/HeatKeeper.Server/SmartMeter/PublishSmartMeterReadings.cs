using HeatKeeper.Server.Events;

namespace HeatKeeper.Server.SmartMeter;

[RequireBackgroundRole]
public record PublishSmartMeterReadingsCommand;

public class PublishSmartMeterReadings(IDbConnection dbConnection, ISqlProvider sqlProvider, IEventBus eventBus) : ICommandHandler<PublishSmartMeterReadingsCommand>
{
    public async Task HandleAsync(PublishSmartMeterReadingsCommand command, CancellationToken cancellationToken = default)
    {
        var readings = (await dbConnection.ReadAsync<SmartMeterReadings>(cancellationToken, sqlProvider.GetSmartMeterReadings)).Single();
        await eventBus.PublishAsync(readings, cancellationToken);
    }
}
