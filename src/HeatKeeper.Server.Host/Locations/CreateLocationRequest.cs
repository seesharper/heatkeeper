using HeatKeeper.Server.Locations;

namespace HeatKeeper.Server.Host.Locations
{
    public class CreateLocationRequest
    {
        public CreateLocationRequest(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}