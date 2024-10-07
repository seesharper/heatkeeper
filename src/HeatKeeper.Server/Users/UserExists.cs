namespace HeatKeeper.Server.Users;

public class UserExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<UserExistsQuery, bool>
{
    public async Task<bool> HandleAsync(UserExistsQuery query, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteScalarAsync<bool>(sqlProvider.UserExists, query);
}

[RequireUserRole]
public record UserExistsQuery(string Email, long Id = 0) : IQuery<bool>;
