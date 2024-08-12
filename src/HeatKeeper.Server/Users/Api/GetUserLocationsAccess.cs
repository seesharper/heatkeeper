namespace HeatKeeper.Server.Users.Api;

[RequireAdminRole]
[Get("/api/users/{UserId}/locations-access")]
public record GetUserLocationsAccessQuery(long userId) : IQuery<UserLocationAccess[]>;

public class GetUserLocationsAccess(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetUserLocationsAccessQuery, UserLocationAccess[]>
{
    public async Task<UserLocationAccess[]> HandleAsync(GetUserLocationsAccessQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<UserLocationAccess>(sqlProvider.GetUserLocationsAccess, query)).ToArray();
}

public record UserLocationAccess(long LocationId, string LocationName, bool HasAccess);