using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using DbReader;
using CQRS.Query.Abstractions;

namespace HeatKeeper.Server.Locations
{
    public class LocationExistsQueryHandler : IQueryHandler<LocationExistsQuery, bool>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public LocationExistsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task<bool> HandleAsync(LocationExistsQuery query, CancellationToken cancellationToken = default)
        {
            return (await dbConnection.ExecuteScalarAsync<long>(sqlProvider.LocationExists, query)) == 1 ? true : false;
        }
    }
    public class LocationExistsQuery : IQuery<bool>
    {
        public LocationExistsQuery(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }
        public string Name { get; }
    }




}
