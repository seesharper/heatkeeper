using System.Collections.Generic;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Sensors
{
    [RequireReporterRole]
    public class CreateMissingSensorsCommand
    {
        public CreateMissingSensorsCommand(IEnumerable<string> externalSensorIds)
        {
            ExternalSensorIds = externalSensorIds;
        }

        public IEnumerable<string> ExternalSensorIds { get; }
    }
}
