using HeatKeeper.Server.CQRS;

namespace HeatKeeper.Server.Zones
{
    public class AllZonesQuery : IQuery<ZoneQueryResult[]>
    {
        public AllZonesQuery()
        {
        }
    }
}