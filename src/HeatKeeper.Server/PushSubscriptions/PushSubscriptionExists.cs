using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.PushSubscriptions;

[RequireUserRole]
public record PushSubscriptionExistsQuery(string Endpoint) : IQuery<bool>;

public class PushSubscriptionExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<PushSubscriptionExistsQuery, bool>
{
    public Task<bool> HandleAsync(PushSubscriptionExistsQuery query, CancellationToken cancellationToken = default)
    {
        return dbConnection.ExecuteScalarAsync<bool>(sqlProvider.PushSubscriptionExists, query);
    }
}
