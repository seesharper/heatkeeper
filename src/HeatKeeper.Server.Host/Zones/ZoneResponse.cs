namespace HeatKeeper.Server.Host.Zones
{
    public class ZoneResponse
    {
        public ZoneResponse(long id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public long Id { get; }
        public string Name { get; }
        public string Description { get; }
    }
}