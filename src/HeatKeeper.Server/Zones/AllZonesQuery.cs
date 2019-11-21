using CQRS.Query.Abstractions;

namespace HeatKeeper.Server.Zones
{
    public class AllZonesQuery : IQuery<ZoneQueryResult[]>
    {
        public AllZonesQuery()
        {
        }
    }
}