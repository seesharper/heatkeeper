namespace HeatKeeper.Server.Host.Users
{
    public class AuthenticateUserResponse
    {
        public AuthenticateUserResponse(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}