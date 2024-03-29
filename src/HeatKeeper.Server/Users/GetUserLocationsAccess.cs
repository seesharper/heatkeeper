using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Users;

[RequireAdminRole]
public record GetUserLocationsAccessQuery(long userId) : IQuery<UserLocationAccess[]>;

public record UserLocationAccess(long LocationId, string LocationName, bool HasAccess);

public class GetUserLocationsAccessQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetUserLocationsAccessQuery, UserLocationAccess[]>
{
    public async Task<UserLocationAccess[]> HandleAsync(GetUserLocationsAccessQuery query, CancellationToken cancellationToken = default)
    {
        return (await dbConnection.ReadAsync<UserLocationAccess>(sqlProvider.GetUserLocationsAccess, query)).ToArray();
    }
}