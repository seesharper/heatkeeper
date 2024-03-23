using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record ActivateProgramCommand(long ProgramId);

public class ActivateProgram(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<ActivateProgramCommand>
{
    public async Task HandleAsync(ActivateProgramCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.ActivateProgram, command);
}