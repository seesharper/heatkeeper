using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server
{
    public abstract class MultiRecordDatabaseQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult[]> where TQuery : IQuery<TResult[]>
    {
        private readonly IDbConnection dbConnection;
        protected readonly ISqlProvider SqlProvider;

        public MultiRecordDatabaseQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.SqlProvider = sqlProvider;
        }

        public virtual async Task<TResult[]> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var sqlPropertyName = typeof(TQuery).ReflectedType.Name;
            var sql = (string)typeof(ISqlProvider).GetProperty(sqlPropertyName).GetValue(SqlProvider);
            return (await dbConnection.ReadAsync<TResult>(sql, query)).ToArray();
        }
    }

}