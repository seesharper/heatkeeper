namespace HeatKeeper.Server.Measurements;

[RequireBackgroundRole]
public record DeleteExpiredMeasurementsCommand();

public class DeleteExpiredMeasurementsCommandHandler(TimeProvider timeProvider, ICommandExecutor commandExecutor) : ICommandHandler<DeleteExpiredMeasurementsCommand>
{
    public async Task HandleAsync(DeleteExpiredMeasurementsCommand command, CancellationToken cancellationToken = default)
    {
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        var hourExpired = utcNow.AddHours(-1);
        var dayExpired = utcNow.AddDays(-1);
        var weekExpired = utcNow.AddDays(-7);

        await commandExecutor.ExecuteAsync(new DeleteMeasurementsCommand(hourExpired, RetentionPolicy.Hour), cancellationToken);
        await commandExecutor.ExecuteAsync(new DeleteMeasurementsCommand(dayExpired, RetentionPolicy.Day), cancellationToken);
        await commandExecutor.ExecuteAsync(new DeleteMeasurementsCommand(weekExpired, RetentionPolicy.Week), cancellationToken);
    }
}
