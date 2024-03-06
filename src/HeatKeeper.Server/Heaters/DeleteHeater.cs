using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Heaters;

[RequireAdminRole]
public record DeleteHeaterCommand(long HeaterId);

public class DeleteHeater(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteHeaterCommand>
{
    public async Task HandleAsync(DeleteHeaterCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteHeater, command);
}