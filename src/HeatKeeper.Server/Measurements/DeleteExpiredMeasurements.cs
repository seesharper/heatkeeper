namespace HeatKeeper.Server.Measurements;

[RequireBackgroundRole]
public record DeleteMeasurementsCommand(DateTime RetentionDate, RetentionPolicy RetentionPolicy);

public class DeleteMeasurementsCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteMeasurementsCommand>
{
    public async Task HandleAsync(DeleteMeasurementsCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteExpiredMeasurements, command);
}