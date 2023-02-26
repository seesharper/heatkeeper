using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

public interface IScheduleCommand
{
    string Name { get; }

    string CronExpression { get; }
}

[RequireUserRole]
public record CreateScheduleCommand(long ProgramId, string Name, string CronExpression) : IScheduleCommand
{
    public long ScheduleId { get; set; }
}

public class CreateScheduleCommandHandler : ICommandHandler<CreateScheduleCommand>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public CreateScheduleCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(CreateScheduleCommand command, CancellationToken cancellationToken = default)
    {
        await _dbConnection.ExecuteAsync(_sqlProvider.InsertSchedule, command);
        command.ScheduleId = await _dbConnection.ExecuteScalarAsync<long>(_sqlProvider.GetLastInsertedRowId);
    }
}