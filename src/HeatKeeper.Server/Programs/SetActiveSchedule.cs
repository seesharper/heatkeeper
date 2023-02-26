using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireBackgroundRole]
public record SetActiveScheduleCommand(long ProgramId, long ScheduleId);

public class SetActiveScheduleCommandHandler : ICommandHandler<SetActiveScheduleCommand>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public SetActiveScheduleCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(SetActiveScheduleCommand command, CancellationToken cancellationToken = default)
        => await _dbConnection.ExecuteAsync(_sqlProvider.SetActiveSchedule, command);
}