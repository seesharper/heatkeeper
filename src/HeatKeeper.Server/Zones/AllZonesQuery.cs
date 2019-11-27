using CQRS.Query.Abstractions;

namespace HeatKeeper.Server.Zones
{
    public class AllZonesQuery : IQuery<Zone[]>
    {
        public AllZonesQuery()
        {
        }
    }
}
