using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Zones
{
    public static class ZonesByLocation
    {
        public class Handler : MultiRecordDatabaseQueryHandler<Query, Result>
        {
            public Handler(IDbConnection dbConnection, ISqlProvider sqlProvider) : base(dbConnection, sqlProvider) { }
        }

        [RequireUserRole]
        public record Query(long LocationId) : IQuery<Result[]>;

        public record Result(long Id, string Name, string Description);
    }


}
