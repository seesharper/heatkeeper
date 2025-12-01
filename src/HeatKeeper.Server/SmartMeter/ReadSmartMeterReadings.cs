using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.SmartMeter;

[RequireUserRole]
public record ReadSmartMeterReadingsQuery() : IQuery<SmartMeterReadings>;

public class ReadSmartMeterReadings(Func<IDbConnection> dbConnectionFactory, ISqlProvider sqlProvider, ILogger<ReadSmartMeterReadings> logger) : IQueryHandler<ReadSmartMeterReadingsQuery, SmartMeterReadings>
{
    public async Task<SmartMeterReadings> HandleAsync(ReadSmartMeterReadingsQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Reading smart meter readings from database.");
        using var dbConnection = dbConnectionFactory();
        var readings = (await dbConnection.ReadAsync<SmartMeterReadings>(cancellationToken, sqlProvider.GetSmartMeterReadings)).Single();
        return readings;
    }
}