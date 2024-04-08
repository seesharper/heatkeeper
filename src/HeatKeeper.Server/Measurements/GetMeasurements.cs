namespace HeatKeeper.Server.Measurements;


[RequireUserRole]
public record GetMeasurementsQuery(long zoneId, DateTime since, MeasurementType measurementType) : IQuery<Measurement[]>;

public class GetMeasurementsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetMeasurementsQuery, Measurement[]>
{
    public async Task<Measurement[]> HandleAsync(GetMeasurementsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<Measurement>(sqlProvider.GetMeasurements, query)).ToArray();
}


public record Measurement(double Value, DateTime Created);
