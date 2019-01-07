using HeatKeeper.Server.CQRS;

namespace HeatKeeper.Server.Zones
{
    public class GetAllZonesQuery : IQuery<ZoneQueryResult[]>
    {
        public GetAllZonesQuery(string location)
        {
            Location = location;
        }

        public string Location { get; }
    }
}