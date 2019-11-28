using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Users
{
    public class GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserQueryResult>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public GetUserQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<GetUserQueryResult> HandleAsync(GetUserQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ReadAsync<GetUserQueryResult>(sqlProvider.GetUser, query)).SingleOrDefault();
        }
    }

    [RequireNoRole]
    public class GetUserQuery : IQuery<GetUserQueryResult>
    {
        public GetUserQuery(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }

    public class GetUserQueryResult
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsAdmin { get; set; }

        public string HashedPassword { get; set; }
    }


}
