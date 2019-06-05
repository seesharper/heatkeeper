namespace HeatKeeper.Server.Zones
{
    public class InsertZoneCommand : ZoneCommand
    {
        public InsertZoneCommand(string name, string description, long locationId) : base(name, description, locationId)
        {
        }
    }
}