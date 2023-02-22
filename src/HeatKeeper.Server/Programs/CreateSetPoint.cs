using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record CreateSetPointCommand(long ScheduleId, double Value, double Hysteresis)
{
    public long SetPointId { get; set; }
}


public class CreateSetPointCommandHandler : ICommandHandler<CreateSetPointCommand>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public CreateSetPointCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(CreateSetPointCommand command, CancellationToken cancellationToken = default)
    {
        await _dbConnection.ExecuteAsync(_sqlProvider.InsertSetPoint, command);
        command.SetPointId = await _dbConnection.ExecuteScalarAsync<long>(_sqlProvider.GetLastInsertedRowId);
    }

}
