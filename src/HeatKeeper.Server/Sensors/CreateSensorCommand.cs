using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Sensors
{
    [RequireReporterRole]
    public class CreateSensorCommand
    {
        public CreateSensorCommand(string externalId, string name, string description)
        {
            ExternalId = externalId;
            Name = name;
            Description = description;
        }

        public string ExternalId { get; }
        public string Name { get; }
        public string Description { get; }
    }
}
