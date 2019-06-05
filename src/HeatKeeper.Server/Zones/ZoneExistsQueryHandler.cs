using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;

namespace HeatKeeper.Server.Zones
{
    public class ZoneExistsQueryHandler : IQueryHandler<ZoneExistsQuery,bool>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public ZoneExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<bool> HandleAsync(ZoneExistsQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.ZoneExists, query)) == 1 ? true : false;
        }
    }
    public class ZoneExistsQuery : IQuery<bool>
    {
        public ZoneExistsQuery(long id, long locationId, string name)
        {
            Id = id;
            LocationId = locationId;
            Name = name;
        }

        public long Id { get; }
        public long LocationId { get; }
        public string Name { get; }
    }



}