namespace HeatKeeper.Server.Zones
{
    public class CreateZoneCommand
    {
        public CreateZoneCommand(string name, string description, string location)
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