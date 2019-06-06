namespace HeatKeeper.Server.Locations
{
    public class InsertLocationCommand
    {
        public InsertLocationCommand(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public long Id { get; set;}
    }
}