namespace HeatKeeper.Server.Host.Locations
{
    public class AddUserRequest
    {
        public AddUserRequest(long userId, long locationId)
        {
            UserId = userId;
            LocationId = locationId;
        }

        public long UserId { get; }
        public long LocationId { get; }
    }
}