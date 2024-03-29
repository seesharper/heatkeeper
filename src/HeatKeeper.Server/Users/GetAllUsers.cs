using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Users
{
    public class AllUsersQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<AllUsersQuery, UserInfo[]>
    {
        public async Task<UserInfo[]> HandleAsync(AllUsersQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ReadAsync<UserInfo>(sqlProvider.GetAllUsers)).ToArray();
        }
    }

    [RequireUserRole]
    public record AllUsersQuery : IQuery<UserInfo[]>;

    public record UserInfo(long Id, string Email, string FirstName, string LastName, bool IsAdmin);
}
