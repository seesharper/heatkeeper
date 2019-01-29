namespace HeatKeeper.Server.Host.Locations
{
    public class AddUserResponse
    {
        public AddUserResponse(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}