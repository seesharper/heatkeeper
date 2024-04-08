namespace HeatKeeper.Server.Zones
{
    public class ZoneDetailsQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<ZoneDetailsQuery, ZoneDetails>
    {
        public async Task<ZoneDetails> HandleAsync(ZoneDetailsQuery query, CancellationToken cancellationToken = default)
            => (await dbConnection.ReadAsync<ZoneDetails>(sqlProvider.GetZoneDetails, query)).Single();
    }

    [RequireAdminRole]
    public record ZoneDetailsQuery(long ZoneId) : IQuery<ZoneDetails>;


    public class ZoneDetails
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string MqttTopic { get; set; }

        public bool IsDefaultOutsideZone { get; set; }

        public bool IsDefaultInsideZone { get; set; }

        public long LocationId { get; set; }
    }
}
