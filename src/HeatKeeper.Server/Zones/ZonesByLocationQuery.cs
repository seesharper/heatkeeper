using HeatKeeper.Abstractions.CQRS;

namespace HeatKeeper.Server.Zones
{
    public class ZonesByLocationQuery : IQuery<ZoneQueryResult[]>
    {
        public ZonesByLocationQuery(string location)
        {
            Location = location;
        }

        public string Location { get; }
    }
}