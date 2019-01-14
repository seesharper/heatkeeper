using System.Collections.Generic;

namespace HeatKeeper.Server.Sensors
{
    public class CreateMissingSensorsCommand
    {
        public CreateMissingSensorsCommand(IEnumerable<string> externalSensorIds)
        {
            ExternalSensorIds = externalSensorIds;
        }

        public IEnumerable<string> ExternalSensorIds { get; }
    }
}