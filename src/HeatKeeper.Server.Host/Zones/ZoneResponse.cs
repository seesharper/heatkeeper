namespace HeatKeeper.Server.Zones
{
    public class ZoneResponse
    {
        public ZoneResponse(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; }
        public string Description { get; }
    }
}