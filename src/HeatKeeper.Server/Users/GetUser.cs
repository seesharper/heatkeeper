namespace HeatKeeper.Server.Users;

[RequireAnonymousRole]
public record GetUserQuery(string Email) : IQuery<GetUserQueryResult>;

public class GetUser(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetUserQuery, GetUserQueryResult>
{
    public async Task<GetUserQueryResult> HandleAsync(GetUserQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<GetUserQueryResult>(sqlProvider.GetUser, query)).SingleOrDefault();
}

public record GetUserQueryResult(long Id, string Email, string FirstName, string LastName, bool IsAdmin, string HashedPassword);
