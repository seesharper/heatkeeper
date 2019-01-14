namespace HeatKeeper.Server.Zones
{
    public class ZoneQueryResult
    {
        public ZoneQueryResult(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; }
        public string Description { get; }
    }
}