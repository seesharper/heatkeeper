namespace HeatKeeper.Server.Host.Locations
{
    public class CreateLocationResponse
    {
        public CreateLocationResponse(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}