using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Heaters;

[RequireAdminRole]
public record UpdateHeaterCommand(long HeaterId, string Name, string Description, string MqttTopic, string OnPayload, string OffPayload);

public class UpdateHeater(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<UpdateHeaterCommand>
{
    public async Task HandleAsync(UpdateHeaterCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateHeater, command);
}

