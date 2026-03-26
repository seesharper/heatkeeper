using System.Data;
using System.Runtime.CompilerServices;
using HeatKeeper.Server.Database;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server.Locations.Api;

[RequireUserRole]
[Get("api/locations/{LocationId}/energy-consumers")]
public record GetEnergyConsumersQuery(long LocationId, bool ShowAll = false)
    : IQuery<ServerSentEventsResult<EnergyConsumer[]>>;

public record EnergyConsumer(long SensorId, string SensorName, double ActivePowerImport, DateTime Updated);

public class GetEnergyConsumersQueryHandler(IQueryExecutor queryExecutor)
    : IQueryHandler<GetEnergyConsumersQuery, ServerSentEventsResult<EnergyConsumer[]>>
{
    public async Task<ServerSentEventsResult<EnergyConsumer[]>> HandleAsync(
        GetEnergyConsumersQuery query, CancellationToken cancellationToken = default)
        => await Task.FromResult(TypedResults.ServerSentEvents(StreamAsync(query, cancellationToken)));

    private async IAsyncEnumerable<EnergyConsumer[]> StreamAsync(
        GetEnergyConsumersQuery query, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return await ReadFromDatabase(query);
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(10000, cancellationToken);
            yield return await ReadFromDatabase(query);
        }
    }

    private async Task<EnergyConsumer[]> ReadFromDatabase(GetEnergyConsumersQuery query)
    {
        var consumers = await queryExecutor.ExecuteAsync(new ReadEnergyConsumersQuery(query.LocationId));
        return query.ShowAll ? consumers : consumers.Where(c => c.ActivePowerImport > 0).ToArray();
    }
}

[RequireBackgroundRole]
public record ReadEnergyConsumersQuery(long LocationId) : IQuery<EnergyConsumer[]>;

public class ReadEnergyConsumersQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    : IQueryHandler<ReadEnergyConsumersQuery, EnergyConsumer[]>
{
    public async Task<EnergyConsumer[]> HandleAsync(ReadEnergyConsumersQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<EnergyConsumer>(sqlProvider.GetEnergyConsumers, query)).ToArray();
}
