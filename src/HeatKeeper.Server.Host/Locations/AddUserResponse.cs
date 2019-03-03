namespace HeatKeeper.Server.Host.Locations
{
    public class AddUserLocationResponse
    {
        public AddUserLocationResponse(long userLocationId)
        {
            UserLocationId = userLocationId;
        }

        public long UserLocationId { get; }
    }
}