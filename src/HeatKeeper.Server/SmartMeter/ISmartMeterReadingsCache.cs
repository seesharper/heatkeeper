namespace HeatKeeper.Server.SmartMeter;


public interface ISmartMeterReadingsCache
{
    Task<SmartMeterReadings> Get(IQueryHandler<ReadSmartMeterReadingsQuery, SmartMeterReadings> handler, CancellationToken cancellationToken = default);
    void Invalidate();
}

public class SmartMeterReadingsCache() : ISmartMeterReadingsCache
{
    private SmartMeterReadings _cachedReadings;
    public async Task<SmartMeterReadings> Get(IQueryHandler<ReadSmartMeterReadingsQuery, SmartMeterReadings> handler, CancellationToken cancellationToken = default)
    {
        if (_cachedReadings != null)
        {
            return _cachedReadings;
        }

        _cachedReadings = await handler.HandleAsync(new ReadSmartMeterReadingsQuery(), cancellationToken);
        return _cachedReadings;
    }

    public void Invalidate() => _cachedReadings = null;
}