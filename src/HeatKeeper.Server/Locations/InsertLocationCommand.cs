using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Locations
{
    [RequireAdminRole]
    public class InsertLocationCommand
    {
        public InsertLocationCommand(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public long Id { get; set; }
    }
}
