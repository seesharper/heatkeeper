using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Zones
{
    [RequireUserRole]
    public class InsertZoneCommand : ZoneCommand
    {
        public InsertZoneCommand(string name, string description, long locationId) : base(name, description, locationId)
        {
        }
    }
}
