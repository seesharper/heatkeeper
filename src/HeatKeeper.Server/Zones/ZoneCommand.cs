namespace HeatKeeper.Server.Zones
{
    public class ZoneCommand
    {
        public ZoneCommand(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }
        public long LocationId { get; set; }
        public long Id { get; set; }
    }
}
