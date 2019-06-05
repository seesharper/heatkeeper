using HeatKeeper.Abstractions.CQRS;

namespace HeatKeeper.Server.Zones
{
    public class ZonesByLocationQuery : IQuery<ZoneQueryResult[]>
    {
        public ZonesByLocationQuery(long locationId)
        {
            LocationId = locationId;
        }

        public long LocationId { get; }
    }
}