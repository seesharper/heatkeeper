using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;

namespace HeatKeeper.Server.Measurements
{
    public class TestQueryHandler : IQueryHandler<TestQuery,TestQueryResult>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public TestQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<TestQueryResult> HandleAsync(TestQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }
    }
    public class TestQuery : IQuery<TestQueryResult>
    {
    }

    public class TestQueryResult
    {
    }


}