using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Heaters;

[RequireUserRole]
public record HeaterDetailsQuery(long HeaterId) : IQuery<HeaterDetails>;

public record HeaterDetails(long Id, string Name, string ZoneName, string Description, string MqttTopic, string OnPayload, string OffPayload);

public class GetHeaterDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<HeaterDetailsQuery, HeaterDetails>
{
    public async Task<HeaterDetails> HandleAsync(HeaterDetailsQuery query, CancellationToken cancellationToken = default) =>
        (await dbConnection.ReadAsync<HeaterDetails>(sqlProvider.GetHeaterDetails, query)).Single();
}
