namespace HeatKeeper.Server.Zones
{
    public record ZoneRecord(long LocationId)
    {
        public long ZoneId { get; set; }
    }

    public class ZoneCommand
    {
        public long ZoneId { get; set; }
        public long LocationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefaultInsideZone { get; set; }
        public bool IsDefaultOutsideZone { get; set; }
    }
}
