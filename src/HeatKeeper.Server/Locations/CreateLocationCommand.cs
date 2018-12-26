namespace HeatKeeper.Server.Locations
{
    public class CreateLocationCommand
    {
        public CreateLocationCommand(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }
    }
}