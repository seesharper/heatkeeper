namespace HeatKeeper.Server.EnergyCosts;

public record CumulativeReading(double Value, DateTime Created);

[RequireReporterRole]
public record GetPreviousCumulativeReadingQuery(string ExternalSensorId, DateTime Created) : IQuery<CumulativeReading[]>;

public class GetPreviousCumulativeReadingQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetPreviousCumulativeReadingQuery, CumulativeReading[]>
{
    public async Task<CumulativeReading[]> HandleAsync(GetPreviousCumulativeReadingQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<CumulativeReading>(sqlProvider.GetPreviousCumulativeReading, query)).ToArray();
}
