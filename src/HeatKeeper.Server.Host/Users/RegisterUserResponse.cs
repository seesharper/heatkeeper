namespace HeatKeeper.Server.Host.Users
{
    public class RegisterUserResponse
    {
        public RegisterUserResponse(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}