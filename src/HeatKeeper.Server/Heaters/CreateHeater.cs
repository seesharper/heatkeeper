using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Heaters;

[RequireAdminRole]
public record CreateHeaterCommand(string Name, string Description, string MqttTopic, string OnPayload, string OffPayload, long ZoneId)
{
    public long HeaterId { get; set; }
}

public class CreateHeater(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateHeaterCommand>
{
    public async Task HandleAsync(CreateHeaterCommand command, CancellationToken cancellationToken = default)
    {
        await dbConnection.ExecuteAsync(sqlProvider.InsertHeater, command);
        command.HeaterId = await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetLastInsertedRowId);
    }
}



