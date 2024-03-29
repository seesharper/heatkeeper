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
public record GetUserDetailsQuery(long UserId) : IQuery<UserDetails>;

public class GetUserDetailsQueryHandler : IQueryHandler<GetUserDetailsQuery, UserDetails>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetUserDetailsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<UserDetails> HandleAsync(GetUserDetailsQuery query, CancellationToken cancellationToken = default)
    {
        return (await _dbConnection.ReadAsync<UserDetails>(_sqlProvider.GetUserDetails, query)).Single();
    }
}

public record UserDetails(long Id, string Email, string FirstName, string LastName, bool IsAdmin);
