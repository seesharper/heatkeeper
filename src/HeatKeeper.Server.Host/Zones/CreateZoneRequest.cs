namespace HeatKeeper.Server.WebApi.Zones
{
    public class CreateZoneRequest
    {
        public CreateZoneRequest(string name, string description, string location)
        {
            Name = name;
            Description = description;
            Location = location;
        }

        public string Name { get; }
        public string Description { get; }
        public string Location { get; }
    }

}