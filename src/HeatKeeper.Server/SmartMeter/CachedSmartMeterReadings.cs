
namespace HeatKeeper.Server.SmartMeter;

// Decorator that adds caching to the smart meter readings service
public class CachedSmartMeterReadingsDecorator(IQueryHandler<ReadSmartMeterReadingsQuery, SmartMeterReadings> handler, ISmartMeterReadingsCache smartMeterReadingsCache) : IQueryHandler<ReadSmartMeterReadingsQuery, SmartMeterReadings>
{
    public async Task<SmartMeterReadings> HandleAsync(ReadSmartMeterReadingsQuery query, CancellationToken cancellationToken = default)
    {
        return await smartMeterReadingsCache.Get(handler, cancellationToken);
    }
}