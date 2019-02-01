using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;

namespace HeatKeeper.Server.Users
{
    public class GetUserQueryHandler : IQueryHandler<GetUserQuery,GetUserQueryResult>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public GetUserQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<GetUserQueryResult> HandleAsync(GetUserQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await dbConnection.ReadAsync<GetUserQueryResult>(sqlProvider.GetUser, query)).SingleOrDefault();
        }
    }
    public class GetUserQuery : IQuery<GetUserQueryResult>
    {
        public GetUserQuery(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class GetUserQueryResult
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        public string HashedPassword { get; set; }
    }


}