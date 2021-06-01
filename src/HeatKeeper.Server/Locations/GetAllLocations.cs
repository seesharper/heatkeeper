using System.Data;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Locations
{
    public static class GetAllLocations
    {
        [RequireUserRole]
        public record Query : IQuery<Result[]>;
        public record Result(long Id, string Name, string Description);

        public class Handler : MultiRecordDatabaseQueryHandler<Query, Result>
        {
            public Handler(IDbConnection dbConnection, ISqlProvider sqlProvider) : base(dbConnection, sqlProvider) { }
        }
    }
}