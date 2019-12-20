namespace HeatKeeper.Server.Zones
{
    public class ZoneCommand
    {
        public long ZoneId { get; set; }
        public long LocationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool UseAsDefaultInsideZone { get; set; }
        public bool UseAsDefaultOutsideZone { get; set; }
    }
}
