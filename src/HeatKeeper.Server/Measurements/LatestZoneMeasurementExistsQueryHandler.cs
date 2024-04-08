namespace HeatKeeper.Server.Measurements
{
    public class LatestZoneMeasurementExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<LatestZoneMeasurementExistsQuery, bool>
    {
        public async Task<bool> HandleAsync(LatestZoneMeasurementExistsQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.LatestZoneMeasurementExists, query)) == 1;
    }

    [RequireReporterRole]
    public record LatestZoneMeasurementExistsQuery(long ZoneId, MeasurementType MeasurementType) : IQuery<bool>;
}
