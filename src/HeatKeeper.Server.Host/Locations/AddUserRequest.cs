namespace HeatKeeper.Server.Host.Locations
{
    public class AddUserLocationRequest
    {
        public AddUserLocationRequest(long userId, long locationId)
        {
            UserId = userId;
            LocationId = locationId;
        }

        public long UserId { get; }
        public long LocationId { get; }
    }
}