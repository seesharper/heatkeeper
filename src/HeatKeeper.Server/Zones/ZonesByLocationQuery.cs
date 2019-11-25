using CQRS.Query.Abstractions;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Zones
{
    [RequireUserRole]
    public class ZonesByLocationQuery : IQuery<ZoneQueryResult[]>
    {
        public ZonesByLocationQuery(long locationId)
        {
            LocationId = locationId;
        }

        public long LocationId { get; }
    }
}
