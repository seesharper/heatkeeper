using System.Runtime.CompilerServices;
using CQRS.Execution;
using HeatKeeper.Server.Events;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server.SmartMeter;

[Get("api/smart-meter")]
[RequireUserRole]
public record GetSmartMeterReadingsQuery() : IQuery<ServerSentEventsResult<SmartMeterReadings>>;

[DomainEvent(6, "Smart Meter Reading", "Published every 10 seconds with the latest smart meter data")]
public record SmartMeterReadings(
    double ActivePowerImport,
    double CurrentPhase1,
    double CurrentPhase2,
    double CurrentPhase3,
    double VoltageBetweenPhase1AndPhase2,
    double VoltageBetweenPhase1AndPhase3,
    double VoltageBetweenPhase2AndPhase3,
    double CumulativePowerImport,
    DateTime Timestamp
);

public class GetSmartMeterReadings(IQueryExecutor queryExecutor) : IQueryHandler<GetSmartMeterReadingsQuery, ServerSentEventsResult<SmartMeterReadings>>
{
    public async Task<ServerSentEventsResult<SmartMeterReadings>> HandleAsync(GetSmartMeterReadingsQuery query, CancellationToken cancellationToken = default)
    {
        var readings = GetReadingsAsync(cancellationToken);
        return await Task.FromResult(TypedResults.ServerSentEvents(readings));
    }

    private async IAsyncEnumerable<SmartMeterReadings> GetReadingsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return await GetReadingsFromDatabase();
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(10000, cancellationToken);
            yield return await GetReadingsFromDatabase();
        }
    }

    private async Task<SmartMeterReadings> GetReadingsFromDatabase() 
        => await queryExecutor.ExecuteScopedAsync(new ReadSmartMeterReadingsQuery(), CancellationToken.None);
}

