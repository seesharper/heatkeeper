namespace HeatKeeper.Server.Users.Api;

[RequireUserRole]
[Get("/api/users")]
public record AllUsersQuery : IQuery<UserInfo[]>;

public class AllUsersQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<AllUsersQuery, UserInfo[]>
{
    public async Task<UserInfo[]> HandleAsync(AllUsersQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<UserInfo>(sqlProvider.GetAllUsers)).ToArray();
}

public record UserInfo(long Id, string Email, string FirstName, string LastName, bool IsAdmin);
