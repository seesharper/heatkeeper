namespace HeatKeeper.Server.Zones
{
    public class CreateZoneCommand
    {
        public CreateZoneCommand(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; }
        public string Description { get; }
    }
}