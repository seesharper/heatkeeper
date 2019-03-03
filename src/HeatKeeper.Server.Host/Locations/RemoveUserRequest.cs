namespace HeatKeeper.Server.Host.Locations
{
    public class RemoveUserRequest
    {
        public RemoveUserRequest(long userLocationId)
        {
            UserLocationId = userLocationId;
        }
        public long UserLocationId { get; }
    }
}