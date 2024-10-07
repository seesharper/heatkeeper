namespace HeatKeeper.Server.Users.Api;

[RequireAdminRole]
[Get("/api/users/{UserId}")]
public record GetUserDetailsQuery(long UserId) : IQuery<UserDetails>;

public class GetUserDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetUserDetailsQuery, UserDetails>
{
    public async Task<UserDetails> HandleAsync(GetUserDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<UserDetails>(sqlProvider.GetUserDetails, query)).Single();
}

public record UserDetails(long Id, string Email, string FirstName, string LastName, bool IsAdmin);
