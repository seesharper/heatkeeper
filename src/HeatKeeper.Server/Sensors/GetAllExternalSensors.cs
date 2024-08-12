namespace HeatKeeper.Server.Sensors;

public class GetAllExternalSensorsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetAllExternalSensorsQuery, ExternalSensorQueryResult[]>
{
    public async Task<ExternalSensorQueryResult[]> HandleAsync(GetAllExternalSensorsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ExternalSensorQueryResult>(sqlProvider.GetAllExternalSensors, query)).ToArray();
}

[RequireReporterRole]
public record GetAllExternalSensorsQuery : IQuery<ExternalSensorQueryResult[]>;

public record ExternalSensorQueryResult(string ExternalId);
