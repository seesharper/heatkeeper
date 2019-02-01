namespace HeatKeeper.Server.Host
{
    public class GetLocationsResponse
    {
        public GetLocationsResponse(long id , string name, string description)
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