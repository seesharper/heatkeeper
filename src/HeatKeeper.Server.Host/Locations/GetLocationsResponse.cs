namespace HeatKeeper.Server.Host
{
    public class GetLocationsResponse
    {
        public GetLocationsResponse(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}