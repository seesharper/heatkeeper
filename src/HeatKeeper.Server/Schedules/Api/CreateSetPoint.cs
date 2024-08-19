namespace HeatKeeper.Server.Schedules.Api;


[RequireUserRole]
[Post("api/schedules/{ScheduleId}/setPoints")]
public record CreateSetPointCommand(long ScheduleId, long ZoneId, double Value, double Hysteresis) : PostCommand;

public class CreateSetPointCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateSetPointCommand>
{
    public async Task HandleAsync(CreateSetPointCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertSetPoint, command);
}
