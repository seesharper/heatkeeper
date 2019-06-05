using HeatKeeper.Server.Zones;

namespace HeatKeeper.Server.Host.Zones
{
    public class CreateZoneRequest
    {
        public CreateZoneRequest(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }

}