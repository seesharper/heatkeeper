using HeatKeeper.Abstractions.CQRS;

namespace HeatKeeper.Server.Zones
{
    public class AllZonesQuery : IQuery<ZoneQueryResult[]>
    {
        public AllZonesQuery()
        {
        }
    }
}