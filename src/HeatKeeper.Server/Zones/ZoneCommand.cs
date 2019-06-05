namespace HeatKeeper.Server.Zones
{
    public class ZoneCommand
    {
        public ZoneCommand(string name, string description, long locationId)
        {
            Name = name;
            Description = description;
            LocationId = locationId;
        }

        public string Name { get; }

        public string Description { get; }
        public long LocationId { get; }
        public long Id { get; set;}
    }
}