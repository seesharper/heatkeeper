namespace HeatKeeper.Server.WebApi.Zones
{
    public class CreateZoneRequest
    {
        public CreateZoneRequest(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; }
        public string Description { get; }
    }

}